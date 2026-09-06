window.mapaEntregas = (function () {
    let mapa = null;
    let camadaCaminho = null;

    function inicializar(elementId, nos, arestas) {
        destruir();

        mapa = L.map(elementId).setView([-22.2, -47.6], 7);

        L.tileLayer("https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png", {
            maxZoom: 18,
            attribution: "&copy; OpenStreetMap"
        }).addTo(mapa);

        const porId = {};
        nos.forEach(n => { porId[n.id] = n; });

        arestas.forEach(a => {
            const o = porId[a.origem];
            const d = porId[a.destino];
            if (o && d) {
                L.polyline([[o.lat, o.lng], [d.lat, d.lng]], {
                    color: "#9aa5b1", weight: 2, opacity: 0.6
                }).addTo(mapa);
            }
        });

        nos.forEach(n => {
            const centro = n.tipo === "CentroDistribuicao";
            const emoji = centro ? "🏭" : "📦";
            L.marker([n.lat, n.lng], { title: n.nome, icon: iconeTexto(emoji) })
                .addTo(mapa)
                .bindPopup(`${emoji} <strong>${n.nome}</strong>`);
        });
    }

    function desenharCaminho(coords) {
        if (!mapa) return;

        if (camadaCaminho) {
            mapa.removeLayer(camadaCaminho);
            camadaCaminho = null;
        }

        if (!coords || coords.length === 0) return;

        camadaCaminho = L.polyline(coords, { color: "#e8590c", weight: 5 }).addTo(mapa);
        mapa.fitBounds(camadaCaminho.getBounds(), { padding: [40, 40] });
    }

    function destruir() {
        if (mapa) {
            mapa.remove();
            mapa = null;
        }
        camadaCaminho = null;
    }

    function iconeTexto(texto) {
        return L.divIcon({
            html: `<div style="font-size:20px;line-height:20px">${texto}</div>`,
            className: "marcador-emoji",
            iconSize: [20, 20],
            iconAnchor: [10, 10]
        });
    }

    return { inicializar, desenharCaminho, destruir };
})();
