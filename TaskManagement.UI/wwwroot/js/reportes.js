//#region Configuración

const reporteConfig = {
    endpoint: 'reports/pending-tasks'
};

//#endregion

//#region Selectores

const reporteSelectors = {
    tabla: '#tblReportePendientes',
    tablaBody: '#tblReportePendientes tbody',
    contenedor: '#contenedorReporte',
    loading: '#loadingReporte',
    sinResultados: '#sinResultadosReporte',
    totalPendientes: '#totalPendientes',
    totalVencidas: '#totalVencidas',
    btnActualizar: '#btnActualizarReporte'
};

//#endregion

//#region Manager

const ReportesManager = {
    init: function () {
        this.registrarEventos();
        this.cargarReporte();
    },

    registrarEventos: function () {
        const btnActualizar = document.querySelector(reporteSelectors.btnActualizar);

        if (btnActualizar) { btnActualizar.addEventListener('click', () => this.cargarReporte()); }
    },

    cargarReporte: async function () {
        try {

            $('#loading').show();

            this.mostrarSinResultados(false);

            const response = await api.get(
                reporteConfig.endpoint
            );

            const datos = Array.isArray(response.data)
                ? response.data
                : [];

            this.renderizarTabla(datos);
            this.renderizarTotales(datos);
            this.mostrarSinResultados(datos.length === 0);

        } catch (error) {
            console.error('Error al consultar el reporte:', error);

            this.limpiarReporte();

            mostrarAlerta(null, error?.response?.message ?? error?.message ?? 'No fue posible consultar el reporte.', 'error');

        } finally {
            setTimeout(() => {
                $('#loading').hide();
            }, 200);
        }
    },

    renderizarTabla: function (datos) {
        const tbody = document.querySelector(reporteSelectors.tablaBody);

        if (!tbody) return;

        tbody.innerHTML = datos.map(item => {

            const pendientes = Number(item.totalPendientes ?? 0);
            const vencidas = Number(item.totalVencidas ?? 0);

            return `
                <tr>
                    <td>
                        <div class="reporte-usuario">

                            <div class="reporte-avatar">
                                ${obtenerInicialUsuario(item.usuario)}
                            </div>

                            <div>
                                <span class="reporte-usuario-nombre">
                                    ${escapeHtml(item.usuario)}
                                </span>

                                <small class="reporte-usuario-id">
                                    Usuario #${Number(item.idUsuario ?? 0)}
                                </small>
                            </div>

                        </div>
                    </td>

                    <td class="text-center">
                        ${obtenerBadgePendientes(pendientes)}
                    </td>

                    <td class="text-center">
                        ${obtenerBadgeVencidas(vencidas)}
                    </td>

                    <td class="text-center">
                        ${obtenerBadgeSituacion(pendientes, vencidas)}
                    </td>
                </tr>
            `;
        }).join('');
    },

    renderizarTotales: function (datos) {
        const totalPendientes = datos.reduce(
            (total, item) =>
                total + Number(item.totalPendientes ?? 0),
            0
        );

        const totalVencidas = datos.reduce(
            (total, item) =>
                total + Number(item.totalVencidas ?? 0),
            0
        );

        const elementoPendientes = document.querySelector(reporteSelectors.totalPendientes);
        const elementoVencidas = document.querySelector(reporteSelectors.totalVencidas);

        if (elementoPendientes) {
            elementoPendientes.textContent = totalPendientes;
        }

        if (elementoVencidas) {
            elementoVencidas.textContent = totalVencidas;
        }
    },

    limpiarReporte: function () {
        const tbody = document.querySelector(reporteSelectors.tablaBody);

        if (tbody) tbody.innerHTML = '';

        const totalPendientes = document.querySelector(reporteSelectors.totalPendientes);
        const totalVencidas = document.querySelector(reporteSelectors.totalVencidas);

        if (totalPendientes) {
            totalPendientes.textContent = '0';
        }

        if (totalVencidas) {
            totalVencidas.textContent = '0';
        }
    },   

    mostrarSinResultados: function (mostrar) {
        const elemento = document.querySelector(reporteSelectors.sinResultados);

        const contenedor = document.querySelector(reporteSelectors.contenedor);

        if (elemento) {
            elemento.classList.toggle('d-none', !mostrar);
        }

        if (contenedor && !this.estaCargando()) {
            contenedor.classList.toggle('d-none', mostrar);
        }
    },

    estaCargando: function () {
        const loading = document.querySelector(reporteSelectors.loading);

        return loading ? !loading.classList.contains('d-none') : false;
    }
};

//#endregion

//#region Badges

function obtenerBadgePendientes(total) {
    return `
        <span class="badge-reporte badge-reporte-pendiente">
            <i class="fas fa-clock"></i>
            ${total}
        </span>
    `;
}

function obtenerBadgeVencidas(total) {
    if (total > 0) {
        return `
            <span class="badge-reporte badge-reporte-vencida">
                <i class="fas fa-triangle-exclamation"></i>
                ${total}
            </span>
        `;
    }

    return `
        <span class="badge-reporte badge-reporte-correcto">
            <i class="fas fa-circle-check"></i>
            0
        </span>
    `;
}

function obtenerBadgeSituacion(pendientes, vencidas) {
    if (vencidas > 0) {
        return `
            <span class="badge-situacion badge-situacion-critica">
                Requiere atención
            </span>
        `;
    }

    if (pendientes > 0) {
        return `
            <span class="badge-situacion badge-situacion-pendiente">
                En seguimiento
            </span>
        `;
    }

    return `
        <span class="badge-situacion badge-situacion-correcta">
            Sin pendientes
        </span>
    `;
}

//#endregion

//#region Utilidades

function obtenerInicialUsuario(usuario) {
    const nombre = String(usuario ?? '').trim();

    if (!nombre) {
        return '?';
    }

    const palabras = nombre
        .split(/\s+/)
        .filter(Boolean);

    if (palabras.length === 1) {
        return palabras[0]
            .charAt(0)
            .toUpperCase();
    }

    return (
        palabras[0].charAt(0) +
        palabras[1].charAt(0)
    ).toUpperCase();
}

//#endregion

//#region Inicio

document.addEventListener('DOMContentLoaded', function () {
    ReportesManager.init();
});

//#endregion