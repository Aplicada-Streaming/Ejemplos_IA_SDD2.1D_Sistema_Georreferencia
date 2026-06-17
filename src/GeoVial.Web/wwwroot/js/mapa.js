// Interop Leaflet para la vista de mapa de un relevamiento (F-10).
// Leaflet (global `L`) se carga por <script> en App.razor; este módulo se importa
// dinámicamente desde el componente Blazor.

export function inicializar(elemento, refDotNet, lat, lng, zoom, marcadores) {
    const mapa = L.map(elemento).setView([lat, lng], zoom);
    L.tileLayer("https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png", {
        maxZoom: 19,
        attribution: "&copy; OpenStreetMap",
    }).addTo(mapa);

    elemento._mapa = mapa;
    elemento._capa = L.layerGroup().addTo(mapa);

    const pines = marcadores || [];
    pines.forEach((m) => agregarPin(elemento, m.latitud, m.longitud, m.descripcion));

    if (pines.length > 0) {
        const grupo = L.featureGroup(pines.map((m) => L.marker([m.latitud, m.longitud])));
        mapa.fitBounds(grupo.getBounds().pad(0.25));
    }

    if (refDotNet) {
        mapa.on("click", (e) =>
            refDotNet.invokeMethodAsync("AlHacerClicEnMapa", e.latlng.lat, e.latlng.lng));
    }

    // Leaflet necesita recalcular el tamaño una vez que el contenedor tiene dimensiones.
    setTimeout(() => mapa.invalidateSize(), 200);
}

function agregarPin(elemento, lat, lng, texto) {
    const marcador = L.marker([lat, lng]).addTo(elemento._capa);
    if (texto) {
        marcador.bindPopup(texto);
    }
    return marcador;
}

export function agregarMarcador(elemento, lat, lng, texto) {
    if (elemento && elemento._capa) {
        agregarPin(elemento, lat, lng, texto);
    }
}

export function destruir(elemento) {
    if (elemento && elemento._mapa) {
        elemento._mapa.remove();
        elemento._mapa = null;
        elemento._capa = null;
    }
}
