# Prueba de adaptadores con dobles de prueba (mocks)

> Nota técnica del proyecto **GeoVial**. Toma como caso real el adaptador de almacenamiento
> compatible con S3 ([S3ObjectStore](../../src/GeoVial.Storage/Providers/S3/S3ObjectStore.cs)) y sus
> pruebas ([S3ObjectStoreTests](../../tests/GeoVial.Storage.Tests/S3ObjectStoreTests.cs)), pero los
> conceptos aplican a cualquier componente que se integre con un servicio externo.

## 1. Resumen

Cuando un componente habla con un sistema externo (una API REST, un servicio de objetos, una cola,
una base de datos), no queremos depender de ese sistema para probar **nuestra** lógica. La técnica
consiste en **aislar el componente bajo prueba** reemplazando a su colaborador externo por un
**doble de prueba** (un objeto que imita la interfaz del colaborador y cuyo comportamiento
controlamos). Cuando el doble además **verifica cómo fue usado** (qué métodos se llamaron, con qué
argumentos), hablamos de un **mock**.

En GeoVial, el `S3ObjectStore` traduce nuestro contrato neutral `IObjectStore` a llamadas del SDK de
AWS (`IAmazonS3`). Para probarlo **sin un S3 real**, sustituimos `IAmazonS3` por un doble y
verificamos que el adaptador haga las llamadas correctas y traduzca bien las respuestas y los
errores.

## 2. Fundamentos

### 2.1. Aislar la unidad bajo prueba (SUT)

- **SUT** (*System Under Test* / *Subject Under Test*): el componente que estamos probando. En el
  ejemplo, `S3ObjectStore`.
- **Colaborador** (*collaborator* / *depended-on component*): aquello con lo que el SUT interactúa.
  En el ejemplo, `IAmazonS3`.

Una prueba unitaria debe fallar **solo** cuando el SUT está mal, no cuando el colaborador externo
está caído, lento o cambia de estado. Por eso reemplazamos al colaborador por un doble determinista.

### 2.2. La costura (seam) y la inversión de dependencias

Para poder sustituir un colaborador, el SUT no debe **construirlo** internamente, sino **recibirlo**
(inyección de dependencias) a través de una **abstracción** (interfaz). Ese punto donde podemos
"cortar" y enchufar otra implementación se llama **costura** (*seam*).

```csharp
// El SUT recibe la abstracción del colaborador (no la crea):
public sealed class S3ObjectStore : IObjectStore
{
    private readonly IAmazonS3 _cliente;            // <- costura: interfaz inyectada
    public S3ObjectStore(IAmazonS3 cliente, S3StorageOptions opciones) { _cliente = cliente; /* ... */ }
}
```

Esto es **Inversión de Dependencias** (la "D" de SOLID): el adaptador depende de la abstracción
`IAmazonS3`, no de una conexión concreta. En producción el contenedor inyecta un `AmazonS3Client`;
en pruebas inyectamos un doble.

### 2.3. Puertos y adaptadores (arquitectura hexagonal)

GeoVial usa el patrón **puertos y adaptadores**:

- **Puerto** = el contrato propio, estable y neutral:
  [IObjectStore](../../src/GeoVial.Storage/Abstractions/IObjectStore.cs).
- **Adaptador** = una implementación del puerto que habla una tecnología concreta:
  `LocalObjectStore` (disco), `MemoryObjectStore` (RAM), `S3ObjectStore` (S3).

Los adaptadores son la **frontera** del sistema. Justo ahí, contra la interfaz del SDK externo, es
donde el mock aporta más valor: prueba la **traducción** entre nuestro contrato y el del proveedor.

### 2.4. ¿Qué se verifica? Estado vs. interacción

- **Verificación por estado** (*state verification*): se ejecuta el SUT y se comprueba el
  **resultado** o el estado final. Ej.: "guardar y luego recuperar devuelve los mismos bytes".
- **Verificación por interacción** (*behavior/interaction verification*): se comprueba **cómo** el
  SUT usó a su colaborador (qué se llamó, con qué argumentos, cuántas veces). Ej.: "al guardar se
  llamó a `PutObjectAsync` con el bucket y la clave correctos". Esto es lo propio de un **mock**.

Un buen conjunto de pruebas combina ambas, pero **prioriza el estado/resultado** y usa la
verificación de interacción solo cuando el efecto no es observable de otro modo (p. ej., "no se debe
borrar si el objeto no existía").

## 3. Definiciones: la taxonomía de dobles de prueba

Siguiendo a Gerard Meszaros (*xUnit Test Patterns*) y a Martin Fowler, "doble de prueba"
(*test double*) es el término paraguas. Sus variantes:

| Doble | Qué es | Para qué sirve |
|---|---|---|
| **Dummy** | Objeto que se pasa pero nunca se usa | Rellenar parámetros obligatorios |
| **Stub** | Devuelve respuestas predefinidas a las llamadas | Controlar las **entradas indirectas** del SUT |
| **Spy** | Stub que además **registra** cómo fue llamado | Inspeccionar interacciones a posteriori |
| **Mock** | Doble con **expectativas** de interacción que se **verifican** | Comprobar las **salidas indirectas** del SUT |
| **Fake** | Implementación **real pero simplificada** | Sustituto funcional liviano (no apto para producción) |

> En el habla cotidiana "mock" se usa como sinónimo de cualquier doble; en rigor, **mock** implica
> verificación de interacción. Las librerías modernas (NSubstitute, Moq) crean un objeto que puede
> actuar como **stub** (cuando le configuramos retornos) y como **mock/spy** (cuando verificamos
> llamadas), según cómo lo usemos.

### 3.1. Mock vs. Fake: dos herramientas distintas

Es la distinción más útil en la práctica, y GeoVial tiene ejemplos de las dos:

- **Mock** (doble configurable, normalmente generado por una librería): ideal para **pruebas
  unitarias** de un adaptador contra una **interfaz** (el SDK). Ej.:
  [S3ObjectStoreTests](../../tests/GeoVial.Storage.Tests/S3ObjectStoreTests.cs) sustituye `IAmazonS3`.
- **Fake** (implementación liviana de verdad): ideal para **pruebas de integración** donde varios
  componentes colaboran y querés comportamiento real sin infraestructura. Ej.:
  [MemoryObjectStore](../../src/GeoVial.Storage/Providers/Memory/MemoryObjectStore.cs) es un
  `IObjectStore` en memoria; lo usan las pruebas de la API para no tocar el disco.

Regla práctica: **mock en el borde** (contra el SDK externo, una llamada por vez) y **fake en el
centro** (cuando querés ejercitar la colaboración real de tu propio código).

## 4. La técnica paso a paso

1. **Identificar la costura**: el SUT debe depender de una **interfaz** del colaborador, inyectada
   por constructor. (Si el SDK no ofreciera interfaz, se introduce un *wrapper* propio para crearla.)
2. **Crear el doble** de esa interfaz con una librería de mocking.
3. **Arrange (preparar)**: configurar los **stubs** (qué devuelve el doble ante ciertas llamadas) y
   construir el SUT con el doble.
4. **Act (actuar)**: invocar el método del SUT.
5. **Assert (verificar)**: comprobar el **resultado** (estado) y, si corresponde, las **interacciones**
   (que el doble fue llamado como se esperaba).

Esta estructura es el patrón **AAA** (*Arrange-Act-Assert*).

### 4.1. Mockear roles, no tipos concretos

Se mockea la **interfaz** que representa un **rol** (`IAmazonS3`, `IObjectStore`), no clases
concretas selladas. Las interfaces son estables y expresan intención; mockear concretos lleva a
pruebas frágiles y a veces es imposible (clases `sealed`).

### 4.2. Mockear lo que controlás (o interfaces estables)

Consejo clásico ("don't mock what you don't own"): mockear interfaces **tuyas** o **estables**.
`IAmazonS3` es una interfaz pública y estable del SDK, pensada justamente para ser sustituida en
pruebas, por eso es seguro mockearla. Para SDKs sin interfaz, conviene envolverlos en un puerto
propio y mockear ese puerto.

## 5. Ejemplos representativos (del repositorio)

Todos usan **xUnit** + **FluentAssertions** + **NSubstitute**. El SUT es `S3ObjectStore` y el
colaborador mockeado es `IAmazonS3`.

### 5.1. Preparación común

```csharp
private const string Bucket = "geovial";
private readonly IAmazonS3 _s3 = Substitute.For<IAmazonS3>();   // el doble
private S3ObjectStore Crear() => new(_s3, new S3StorageOptions { Bucket = Bucket });
```

`Substitute.For<IAmazonS3>()` crea un doble: por defecto, cada método devuelve un valor neutro
(`Task` completada, `null`, `0`...). A partir de ahí lo configuramos según cada prueba.

### 5.2. Stub + verificación de interacción (mock)

Probar que "subir" delega en `PutObjectAsync` con los datos correctos. Combina **estado** (el
`StoredObjectInfo` devuelto) e **interacción** (`Received(1)`):

```csharp
[Fact]
public async Task Save_sube_al_bucket_con_clave_y_tipo()
{
    var store = Crear();
    using var contenido = new MemoryStream(new byte[] { 1, 2, 3, 4 });

    var info = await store.SaveAsync("relevamientos/1/foto", contenido, "image/jpeg");

    info.SizeBytes.Should().Be(4);                       // verificación por estado
    await _s3.Received(1).PutObjectAsync(                 // verificación por interacción
        Arg.Is<PutObjectRequest>(r => r.BucketName == Bucket
            && r.Key == "relevamientos/1/foto"
            && r.ContentType == "image/jpeg"),
        Arg.Any<CancellationToken>());
}
```

- `Arg.Is<T>(predicado)` afirma sobre los argumentos; `Arg.Any<T>()` acepta cualquiera.
- `Received(1)` verifica que se llamó **exactamente una vez**.

### 5.3. Stub de una respuesta

Probar que "recuperar" devuelve el contenido que entrega el SDK:

```csharp
[Fact]
public async Task Get_devuelve_el_contenido()
{
    _s3.GetObjectAsync(Bucket, "k", Arg.Any<CancellationToken>())
       .Returns(new GetObjectResponse { ResponseStream = new MemoryStream(new byte[] { 9, 9 }) });

    var store = Crear();
    await using var s = await store.GetAsync("k");
    // ... se lee 's' y se comprueba que son los bytes 9,9
}
```

`.Returns(...)` es la configuración de **stub**: "cuando te pidan `GetObjectAsync(bucket,"k",…)`,
devolvé esto".

### 5.4. Stub que lanza, para probar el mapeo de errores

El valor real del adaptador es **traducir** el error del proveedor (un 404 de S3) a nuestro error
de dominio (`ObjectNotFoundException`). Forzamos el fallo del colaborador:

```csharp
[Fact]
public async Task Get_inexistente_lanza_ObjectNotFound()
{
    _s3.GetObjectAsync(Bucket, "missing", Arg.Any<CancellationToken>())
       .ThrowsAsync(new AmazonS3Exception("no existe") { StatusCode = HttpStatusCode.NotFound });

    var store = Crear();
    await store.Invoking(s => s.GetAsync("missing"))
               .Should().ThrowAsync<ObjectNotFoundException>();
}
```

### 5.5. Verificación negativa: que algo **no** ocurra

"Borrar un objeto inexistente no debe llamar a `DeleteObjectAsync` y debe devolver `false`":

```csharp
[Fact]
public async Task Delete_inexistente_no_borra_y_devuelve_false()
{
    _s3.GetObjectMetadataAsync(Bucket, "k", Arg.Any<CancellationToken>())
       .ThrowsAsync(new AmazonS3Exception("nf") { StatusCode = HttpStatusCode.NotFound });

    var store = Crear();

    (await store.DeleteAsync("k")).Should().BeFalse();
    await _s3.DidNotReceive().DeleteObjectAsync(
        Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
}
```

`DidNotReceive()` es una verificación de interacción negativa: comprueba una **decisión** del SUT
(no borrar) que de otro modo no sería observable.

## 6. Cuándo mock, cuándo fake, cuándo real

| Nivel | Qué prueba | Colaborador externo | Ejemplo en GeoVial |
|---|---|---|---|
| **Unitaria** | Lógica de un componente aislado | **Mock** del SDK | `S3ObjectStoreTests` mockea `IAmazonS3` |
| **Integración (interna)** | Colaboración entre componentes propios | **Fake** liviano | API + `MemoryObjectStore` en memoria |
| **Integración (externa) / e2e** | Contra el servicio real | **Real** o *test container* | Correr el `S3ObjectStore` contra un **MinIO** real |

> Los **mocks no sustituyen** a una prueba contra el servicio real: garantizan que *llamamos bien*
> al SDK, no que el servicio se comporte como creemos. La pirámide se completa con alguna prueba de
> integración real (idealmente contra un contenedor efímero como MinIO con *Testcontainers*).

## 7. Buenas prácticas

- **AAA** y **una intención por prueba**: un solo motivo de fallo, nombre descriptivo del
  *given/when/then*.
- **Verificá resultados antes que interacciones**. Sobre-verificar llamadas vuelve la prueba
  **frágil** (se rompe ante refactors que no cambian el comportamiento). Verificá interacción solo
  cuando es la salida relevante (efectos sin retorno: borrados, envíos, publicaciones).
- **Mockeá la interfaz del rol**, no clases concretas ni tipos de valor (DTOs, `string`, `record`).
- **No mockees tipos que no controlás** salvo que sean interfaces estables pensadas para pruebas.
- **Evitá el sobre-mocking**: si una prueba necesita configurar muchísimos stubs, suele indicar que
  el SUT tiene demasiadas responsabilidades o demasiados colaboradores.
- **Argumentos explícitos**: usá *matchers* (`Arg.Is`) para fijar lo que importa y `Arg.Any` para lo
  irrelevante; así el fallo señala la causa real.
- **Deterministas y rápidas**: sin red, sin reloj real, sin aleatoriedad; el doble da control total.

### 7.1. Olores (anti-patrones)

- **Pruebas que repiten la implementación**: si la prueba describe paso a paso lo que hace el SUT
  (mock de cada línea), no prueba comportamiento, prueba la transcripción del código.
- **Mock de lo que deberías dejar real**: mockear tu propia lógica de dominio en vez de ejercitarla.
- **Verificar `Received` de todo**: acopla la prueba a detalles internos.

## 8. Herramientas en .NET

| Librería | Estilo | Notas |
|---|---|---|
| **NSubstitute** | API fluida y mínima (`Substitute.For`, `Returns`, `Received`) | Elegida en GeoVial por sintaxis clara y sin fricciones |
| **Moq** | Muy difundida (`new Mock<T>()`, `Setup`, `Verify`) | Potente; revisar versión por el episodio *SponsorLink* (4.20) |
| **FakeItEasy** | API uniforme (`A.Fake<T>()`, `A.CallTo`) | Alternativa cómoda |

GeoVial usa **NSubstitute** (`tests/GeoVial.Storage.Tests`), junto con **xUnit** (runner) y
**FluentAssertions** (aserciones legibles: `Should().Be(...)`, `Should().ThrowAsync<...>()`).

## 9. Checklist para probar un adaptador con mocks

- [ ] El SUT recibe el colaborador por **interfaz** inyectada (hay costura).
- [ ] Mockeo la **interfaz del rol** del colaborador externo.
- [ ] Pruebo el **camino feliz** (delegación correcta + resultado traducido).
- [ ] Pruebo el **mapeo de errores** del proveedor a errores de dominio (stub que lanza).
- [ ] Pruebo **decisiones no observables** con verificación de interacción (`Received`/`DidNotReceive`).
- [ ] Las pruebas no usan red/disco/reloj reales; son deterministas.
- [ ] Reservo una prueba de **integración real** (o con contenedor) para el contrato del servicio.

## 10. Referencias

- Martin Fowler — *Mocks Aren't Stubs* y *Test Double*.
- Gerard Meszaros — *xUnit Test Patterns: Refactoring Test Code* (taxonomía de dobles).
- "Test Pyramid" (M. Cohn / M. Fowler): muchas unitarias, algunas de integración, pocas e2e.
- Documentación de **NSubstitute**, **xUnit** y **FluentAssertions**.

## 11. Control de cambios

| Versión | Fecha | Cambios |
|---|---|---|
| 1.0 | 2026-06-17 | Nota inicial sobre prueba de adaptadores con dobles/mocks, con el adaptador S3 de GeoVial como caso. |
