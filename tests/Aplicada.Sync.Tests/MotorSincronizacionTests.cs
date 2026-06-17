using Aplicada.Sync;
using FluentAssertions;

namespace Aplicada.Sync.Tests;

public sealed class MotorSincronizacionTests
{
    private static CambioLocal Cambio(string id, long orden) => new(id, "marcador", $"carga-{id}", orden);

    /// <summary>Backend de prueba que registra el orden de las operaciones y puede simular cortes.</summary>
    private sealed class BackendFake : IBackendSincronizacion
    {
        public List<string> Log { get; } = [];
        public int FallarTrasSubidas { get; set; } = int.MaxValue;
        public ResultadoBajada Bajada { get; set; } = new([], "marca-1");
        private int _subidas;

        public Task SubirCambioAsync(CambioLocal cambio, CancellationToken ct = default)
        {
            if (_subidas >= FallarTrasSubidas)
            {
                throw new BackendInalcanzableException();
            }

            _subidas++;
            Log.Add($"subir:{cambio.IdCambio}");
            return Task.CompletedTask;
        }

        public Task<ResultadoBajada> BajarAsync(string? marca, CancellationToken ct = default)
        {
            Log.Add($"bajar:{marca ?? "(null)"}");
            return Task.FromResult(Bajada);
        }
    }

    [Fact]
    public async Task CA01_sube_antes_de_bajar_y_vacia_la_cola()
    {
        var almacen = new AlmacenLocalEnMemoria();
        var backend = new BackendFake();
        var motor = new MotorSincronizacion(almacen, backend);

        await motor.EncolarAsync(Cambio("a", 1));
        await motor.EncolarAsync(Cambio("b", 2));

        var resumen = await motor.SincronizarAsync();

        resumen.Subidos.Should().Be(2);
        resumen.EstadoFinal.Should().Be(EstadoSesionSync.Lista);
        backend.Log.Should().Equal("subir:a", "subir:b", "bajar:(null)"); // RN-01: subir antes de bajar
        (await motor.EstadoColaAsync()).Pendientes.Should().Be(0);
    }

    [Fact]
    public async Task CA02_cola_vacia_omite_subida_y_baja()
    {
        var backend = new BackendFake();
        var motor = new MotorSincronizacion(new AlmacenLocalEnMemoria(), backend);

        var resumen = await motor.SincronizarAsync();

        resumen.Subidos.Should().Be(0);
        backend.Log.Should().ContainSingle().Which.Should().StartWith("bajar");
    }

    [Fact]
    public async Task CA03_corte_en_subida_no_baja_y_deja_reanudable()
    {
        var almacen = new AlmacenLocalEnMemoria();
        var backend = new BackendFake { FallarTrasSubidas = 1 };
        var motor = new MotorSincronizacion(almacen, backend);

        await motor.EncolarAsync(Cambio("a", 1));
        await motor.EncolarAsync(Cambio("b", 2));
        await motor.EncolarAsync(Cambio("c", 3));

        var accion = async () => await motor.SincronizarAsync();
        await accion.Should().ThrowAsync<BackendInalcanzableException>();

        (await motor.EstadoColaAsync()).Pendientes.Should().Be(2); // 'a' confirmado, 'b' y 'c' conservados
        motor.Estado.Should().Be(EstadoSesionSync.Reanudable);
        backend.Log.Should().NotContain(l => l.StartsWith("bajar"));
    }

    [Fact]
    public async Task Reanuda_subida_sin_reaplicar_lo_confirmado()
    {
        var almacen = new AlmacenLocalEnMemoria();
        var backend = new BackendFake { FallarTrasSubidas = 1 };
        var motor = new MotorSincronizacion(almacen, backend);
        await motor.EncolarAsync(Cambio("a", 1));
        await motor.EncolarAsync(Cambio("b", 2));
        await motor.EncolarAsync(Cambio("c", 3));
        await ((Func<Task>)(async () => await motor.SincronizarAsync())).Should().ThrowAsync<BackendInalcanzableException>();

        // El backend se recupera: el reintento sube solo lo que quedaba, sin reenviar 'a' (RN-02).
        backend.FallarTrasSubidas = int.MaxValue;
        var resumen = await motor.SincronizarAsync();

        resumen.Subidos.Should().Be(2);
        backend.Log.Should().Equal("subir:a", "subir:b", "subir:c", "bajar:(null)");
        (await motor.EstadoColaAsync()).Pendientes.Should().Be(0);
    }

    [Fact]
    public async Task CU02_reencolar_mismo_identificador_no_duplica()
    {
        var motor = new MotorSincronizacion(new AlmacenLocalEnMemoria(), new BackendFake());

        await motor.EncolarAsync(Cambio("x", 1));
        var tamanio = await motor.EncolarAsync(Cambio("x", 1));

        tamanio.Should().Be(1);
    }

    [Fact]
    public async Task CU02_rechaza_cambio_sin_identificador()
    {
        var motor = new MotorSincronizacion(new AlmacenLocalEnMemoria(), new BackendFake());

        var accion = async () => await motor.EncolarAsync(new CambioLocal("", "marcador", "carga", 1));

        await accion.Should().ThrowAsync<IdentificadorCambioAusenteException>();
        (await motor.EstadoColaAsync()).Pendientes.Should().Be(0);
    }

    [Fact]
    public async Task CA04_aplica_actualizacion_en_conflicto_sin_abortar()
    {
        var almacen = new AlmacenLocalEnMemoria();
        var backend = new BackendFake
        {
            Bajada = new([
                new ActualizacionRemota("r1", "marcador", "carga", EnConflicto: true),
                new ActualizacionRemota("r2", "observacion", "carga", EnConflicto: false),
            ], "marca-2"),
        };
        var motor = new MotorSincronizacion(almacen, backend);

        var resumen = await motor.SincronizarAsync();

        resumen.Bajados.Should().Be(2);
        resumen.EnConflicto.Should().Be(1); // RN-03
        almacen.Aplicadas.Should().Contain(a => a.Id == "r1" && a.EnConflicto);
        (await almacen.ObtenerMarcaAsync()).Should().Be("marca-2");
    }
}
