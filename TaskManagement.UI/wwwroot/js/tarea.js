//#region Configuración

const tareasConfig = {
    endpoints: {
        tareas: 'Tasks',
        prioridades: 'Catalogos/Prioridades',
        estatus: 'Catalogos/Estatus',
        usuarios: 'Catalogos/Usuarios'
    },

    urls: {
        index: '/Tareas/Index',
        crear: '/Tareas/Crear',
        editar: idTarea => `/Tareas/Editar/${idTarea}`
    },

    paginacion: {
        paginaActual: 1,
        tamanioPagina: 20
    }
};

//#endregion

//#region Selectores

const tareasSelectors = {
    // Formulario
    idTarea: '#IdTarea',
    titulo: '#txtTitulo',
    descripcion: '#txtDescripcion',
    prioridad: '#ddlPrioridad',
    estatus: '#ddlEstatus',
    usuarioResponsable: '#ddlUsuarioResponsable',
    fechaInicio: '#txtFechaInicio',
    fechaFinalizacion: '#txtFechaFinalizacion',
    fechaLimite: '#txtFechaLimite',

    // Botones CRUD
    btnGuardar: '#btnGuardar',
    btnActualizar: '#btnActualizar',
    btnEliminar: '#confirmBtn',

    // Listado
    tabla: '#tblTareas',
    tablaBody: '#tblTareas tbody',
    sinResultados: '#sinResultados',
    totalRegistros: '#totalRegistros',
    paginacion: '#paginacion',

    // Filtros
    filtroPrioridad: '#filtroPrioridad',
    filtroEstatus: '#filtroEstatus',
    filtroUsuario: '#filtroUsuario',
    filtroFechaInicial: '#filtroFechaInicial',
    filtroFechaFinal: '#filtroFechaFinal',
    btnBuscar: '#btnBuscar',
    btnLimpiar: '#btnLimpiar',

    // Modal de eliminación
    modalConfirmacion: '#modalConfirmacion',
    modalTituloTarea: '#modalTituloTarea'
};

//#endregion

//#region Manager

const TareasManager = {
    init: function () {
        this.registrarEventos();
        this.inicializarPagina();
    },

    registrarEventos: function () {
        const btnGuardar = document.querySelector(tareasSelectors.btnGuardar);
        const btnActualizar = document.querySelector(tareasSelectors.btnActualizar);
        const btnEliminar = document.querySelector(tareasSelectors.btnEliminar);
        const btnBuscar = document.querySelector(tareasSelectors.btnBuscar);
        const btnLimpiar = document.querySelector(tareasSelectors.btnLimpiar);

        if (btnGuardar) { btnGuardar.addEventListener('click', event => this.agregarTarea(event)); }
        if (btnActualizar) { btnActualizar.addEventListener('click', event => this.actualizarTarea(event)); }
        if (btnEliminar) { btnEliminar.addEventListener('click', event => this.eliminarTarea(event)); }
        if (btnBuscar) { btnBuscar.addEventListener('click', () => { tareasConfig.paginacion.paginaActual = 1; this.cargarTareas(); }); }
        if (btnLimpiar) { btnLimpiar.addEventListener('click', () => this.limpiarFiltros()); }
    },

    inicializarPagina: async function () {
        const esPaginaListado = document.querySelector(tareasSelectors.tabla) !== null;
        const esPaginaCrear = document.querySelector(tareasSelectors.btnGuardar) !== null;
        const esPaginaEditar = document.querySelector(tareasSelectors.btnActualizar) !== null;

        try {

            if (esPaginaListado || esPaginaCrear || esPaginaEditar) {
                await this.cargarCatalogos();
            }

            if (esPaginaListado) {
                await this.cargarTareas();
            }

            if (esPaginaEditar) {
                await this.cargarTareaParaEditar();
            }

            this.configurarFechaLimiteMinima();

        } catch (error) {

            console.error('Error al inicializar la página de tareas:', error);

            mostrarError(error, 'No fue posible inicializar la página.');
        }
    },

    cargarCatalogos: async function () {
        const solicitudes = [];

        const ddlPrioridad =
            document.querySelector(tareasSelectors.prioridad);

        const ddlEstatus =
            document.querySelector(tareasSelectors.estatus);

        const ddlUsuario =
            document.querySelector(tareasSelectors.usuarioResponsable);

        const filtroPrioridad =
            document.querySelector(tareasSelectors.filtroPrioridad);

        const filtroEstatus =
            document.querySelector(tareasSelectors.filtroEstatus);

        const filtroUsuario =
            document.querySelector(tareasSelectors.filtroUsuario);

        if (ddlPrioridad || filtroPrioridad) {
            solicitudes.push(
                api.get(tareasConfig.endpoints.prioridades)
                    .then(response => {
                        const prioridades = response.data ?? [];

                        llenarSelect(
                            tareasSelectors.prioridad,
                            prioridades,
                            'idPrioridad',
                            'nombre'
                        );

                        llenarSelect(
                            tareasSelectors.filtroPrioridad,
                            prioridades,
                            'idPrioridad',
                            'nombre'
                        );
                    })
            );
        }

        if (ddlEstatus || filtroEstatus) {
            solicitudes.push(
                api.get(tareasConfig.endpoints.estatus)
                    .then(response => {
                        const estatus = response.data ?? [];

                        llenarSelect(
                            tareasSelectors.estatus,
                            estatus,
                            'idEstatus',
                            'nombre'
                        );

                        llenarSelect(
                            tareasSelectors.filtroEstatus,
                            estatus,
                            'idEstatus',
                            'nombre'
                        );
                    })
            );
        }

        if (ddlUsuario || filtroUsuario) {
            solicitudes.push(
                api.get(tareasConfig.endpoints.usuarios)
                    .then(response => {
                        const usuarios = response.data ?? [];

                        llenarSelect(
                            tareasSelectors.usuarioResponsable,
                            usuarios,
                            'idUsuario',
                            'nombreCompleto'
                        );

                        llenarSelect(
                            tareasSelectors.filtroUsuario,
                            usuarios,
                            'idUsuario',
                            'nombreCompleto'
                        );
                    })
            );
        }

        await Promise.all(solicitudes);
    },

    cargarTareas: async function () {
        const tbody = document.querySelector(tareasSelectors.tablaBody);

        if (!tbody) return;

        try {

            $('#loading').show();            

            const query = this.construirQuery();

            const response = await api.get(
                `${tareasConfig.endpoints.tareas}?${query}`
            );

            const resultado = response.data ?? {};
            const tareas = resultado.items ?? [];

            this.renderizarTabla(tareas);
            this.renderizarPaginacion(resultado);
            this.renderizarTotal(resultado.totalRecords ?? 0);
            this.mostrarSinResultados(tareas.length === 0);
        } catch (error) {

            console.error('Error al consultar tareas:', error);

            mostrarError(error, 'No fue posible consultar las tareas.');

        } finally {
            setTimeout(() => {
                $('#loading').hide();
            }, 200);
        }
    },

    cargarTareaParaEditar: async function () {

        const idTarea = obtenerNumero(tareasSelectors.idTarea);

        if (!idTarea) {
            mostrarError(null, 'No se encontró el identificador de la tarea.');
            return;
        }

        try {
            $('#loading').show();

            const response = await api.get(
                `${tareasConfig.endpoints.tareas}/${idTarea}`
            );

            const tarea = response.data;

            if (!tarea) {
                mostrarError(null, 'No se encontró la tarea solicitada.');
                return;
            }

            asignarValor(tareasSelectors.titulo, tarea.titulo);

            asignarValor(tareasSelectors.descripcion, tarea.descripcion);

            asignarValor(tareasSelectors.prioridad, tarea.idPrioridad);

            asignarValor(tareasSelectors.estatus, tarea.idEstatus);

            asignarValor(tareasSelectors.usuarioResponsable, tarea.idUsuarioResponsable);

            asignarValor(tareasSelectors.fechaInicio, normalizarFechaInput(tarea.fechaInicio));

            asignarValor(tareasSelectors.fechaFinalizacion, normalizarFechaInput(tarea.fechaFinalizacion));

            asignarValor(tareasSelectors.fechaLimite, normalizarFechaInput(tarea.fechaLimite));

        } catch (error) {

            console.error('Error al consultar la tarea:', error);

            mostrarError(error, 'No fue posible consultar la tarea.');

        } finally {
            setTimeout(() => {
                $('#loading').hide();
            }, 200);
        }
    },

    agregarTarea: async function (event) {

        event?.preventDefault();

        // Validar Formulario
        if (!validarFormularioTarea()) return;        

        // Obtener Datos del Formulario
        const data = obtenerDatosFormularioTarea();

        try {
            $('#loading').show();

            this.deshabilitarBotones(true);

            const response = await api.post(
                tareasConfig.endpoints.tareas,
                data
            );

            mostrarAlerta(null, response.message ?? 'Tarea creada correctamente.', 'success');

            setTimeout(() => {
                window.location.href =
                    tareasConfig.urls.index;
            }, 800);

        } catch (error) {
            console.error('Error al crear la tarea:', error);

            mostrarError(error, 'No fue posible crear la tarea.');

        } finally {

            setTimeout(() => {
                $('#loading').hide();
            }, 200);

            this.deshabilitarBotones(false);
        }
    },

    actualizarTarea: async function (event) {

        event?.preventDefault();

        // Validar Formulario
        if (!validarFormularioTarea()) return;

        const idTarea = obtenerNumero(tareasSelectors.idTarea);

        // Validar ID Tarea
        if (!idTarea) {
            mostrarError(null, 'No se encontró el identificador de la tarea.');
            return;
        }

        const data = {
            ...obtenerDatosFormularioTarea(),
            idTarea
        };

        try {
            $('#loading').show();

            this.deshabilitarBotones(true);

            const response = await api.put(
                `${tareasConfig.endpoints.tareas}/${idTarea}`,
                data
            );

            mostrarAlerta(null, response.message ?? 'Tarea actualizada correctamente.', 'success');

            setTimeout(() => {
                window.location.href =
                    tareasConfig.urls.index;
            }, 800);

        } catch (error) {

            console.error('Error al actualizar la tarea:', error);

            mostrarError(error, 'No fue posible actualizar la tarea.');

        } finally {

            setTimeout(() => {
                $('#loading').hide();
            }, 200);

            this.deshabilitarBotones(false);
        }
    },

    eliminarTarea: async function (event) {
        event?.preventDefault();

        const idTarea = obtenerNumero(tareasSelectors.idTarea);

        // Validar ID Tarea
        if (!idTarea) {
            mostrarError(null, 'No se encontró el identificador de la tarea.');
            return;
        }

        try {
            $('#loading').show();

            this.deshabilitarBotones(true);

            const response = await api.delete(
                `${tareasConfig.endpoints.tareas}/${idTarea}`
            );

            this.cerrarModalConfirmacion();

            mostrarAlerta(null,response.message ?? 'Tarea eliminada correctamente.', 'success');

            if (document.querySelector(tareasSelectors.tabla)) {

                await this.cargarTareas();

            } else {

                setTimeout(() => {
                    window.location.href =
                        tareasConfig.urls.index;
                }, 800);
            }
        } catch (error) {

            console.error('Error al eliminar la tarea:', error);

            mostrarError(error, 'No fue posible eliminar la tarea.');
        } finally {

            setTimeout(() => {
                $('#loading').hide();
            }, 200);

            this.deshabilitarBotones(false);
        }
    },

    limpiarFiltros: function () {
        asignarValor(tareasSelectors.filtroPrioridad, '');
        asignarValor(tareasSelectors.filtroEstatus, '');
        asignarValor(tareasSelectors.filtroUsuario, '');
        asignarValor(tareasSelectors.filtroFechaInicial, '');
        asignarValor(tareasSelectors.filtroFechaFinal, '');

        tareasConfig.paginacion.paginaActual = 1;

        this.cargarTareas();
    },

    construirQuery: function () {
        const params = new URLSearchParams();

        params.set('page', tareasConfig.paginacion.paginaActual);

        params.set('pageSize', tareasConfig.paginacion.tamanioPagina);

        agregarParametroSiExiste(params, 'idPrioridad', obtenerValor(tareasSelectors.filtroPrioridad));

        agregarParametroSiExiste(params, 'idEstatus', obtenerValor(tareasSelectors.filtroEstatus));

        agregarParametroSiExiste(params, 'idUsuarioResponsable', obtenerValor(tareasSelectors.filtroUsuario));

        agregarParametroSiExiste(params, 'fechaInicial', obtenerValor(tareasSelectors.filtroFechaInicial));

        agregarParametroSiExiste(params, 'fechaFinal', obtenerValor(tareasSelectors.filtroFechaFinal));

        return params.toString();
    },

    renderizarTabla: function (tareas) {

        const tbody = document.querySelector(tareasSelectors.tablaBody);

        if (!tbody) return;

        tbody.innerHTML = tareas.map(tarea => `
        <tr>
            <td class="text-center">
                ${escapeHtml(tarea.titulo)}
            </td>

            <td class="text-center">
               ${obtenerBadgePrioridad(tarea.prioridad)}
            </td>

            <td class="text-center">
                ${escapeHtml(tarea.usuarioResponsable)}
            </td>

            <td class="text-center">
                ${obtenerBadgeEstatus(tarea.estatus)}
            </td>

            <td class="text-center">
                ${formatearFecha(tarea.fechaLimite)}
            </td>

            <td class="class="text-center"">

                <a href="${tareasConfig.urls.editar(tarea.idTarea)}"
                   class="btn btn-sm btn-outline-primary">
                    <i class="fas fa-pen-to-square me-1"></i>                
                </a>

                <button type="button"
                        class="btn btn-sm btn-outline-danger"
                        onclick="TareasManager.prepararEliminacion(
                            ${tarea.idTarea},
                            '${escapeJs(tarea.titulo)}'
                        )">
                    <i class="fas fa-trash-can me-1"></i>                    
                </button>

            </td>
            
        </tr>
    `).join('');
    },

    prepararEliminacion: function (idTarea, titulo) {

        asignarValor(tareasSelectors.idTarea, idTarea);

        const modalTitulo = document.querySelector(tareasSelectors.modalTituloTarea);

        if (modalTitulo) {
            modalTitulo.textContent = titulo ?? '';
        }

        const modal = document.querySelector(tareasSelectors.modalConfirmacion);

        if (!modal) return;

        if (window.bootstrap) {
            bootstrap.Modal.getOrCreateInstance(modal).show();
            return;
        }

        if (window.jQuery) {
            $(modal).modal('show');
        }
    },

    renderizarPaginacion: function (resultado) {

        const contenedor = document.querySelector(tareasSelectors.paginacion);

        if (!contenedor) return;

        const totalPages = resultado.totalPages ?? 0;
        const paginaActual = tareasConfig.paginacion.paginaActual;

        contenedor.innerHTML = '';

        if (totalPages <= 1) return;

        const anterior = document.createElement('li');

        anterior.className = `page-item ${paginaActual === 1 ? 'disabled' : ''}`;

        anterior.innerHTML = `
        <button type="button"
                class="page-link">
            Anterior
        </button>
    `;
        anterior
            .querySelector('button')
            ?.addEventListener('click', () => {
                if (paginaActual > 1) {
                    tareasConfig.paginacion.paginaActual--;
                    this.cargarTareas();
                }
            });

        contenedor.appendChild(anterior);

        for (
            let pagina = 1;
            pagina <= totalPages;
            pagina++
        ) {
            const item = document.createElement('li');

            item.className =
                `page-item ${pagina === paginaActual
                    ? 'active'
                    : ''
                }`;

            item.innerHTML = `
            <button type="button"
                    class="page-link">
                ${pagina}
            </button>
        `;

            item
                .querySelector('button')
                ?.addEventListener('click', () => {
                    tareasConfig.paginacion.paginaActual =
                        pagina;

                    this.cargarTareas();
                });

            contenedor.appendChild(item);
        }

        const siguiente = document.createElement('li');

        siguiente.className =
            `page-item ${paginaActual === totalPages
                ? 'disabled'
                : ''
            }`;

        siguiente.innerHTML = `
        <button type="button"
                class="page-link">
            Siguiente
        </button>
    `;

        siguiente
            .querySelector('button')
            ?.addEventListener('click', () => {
                if (paginaActual < totalPages) {
                    tareasConfig.paginacion.paginaActual++;
                    this.cargarTareas();
                }
            });

        contenedor.appendChild(siguiente);
    },

    renderizarTotal: function (total) {
        const elemento = document.querySelector(tareasSelectors.totalRegistros);

        if (elemento) {
            elemento.textContent = `${total} registro(s) encontrado(s)`;
        }
    },

    mostrarSinResultados: function (mostrar) {
        const elemento = document.querySelector(tareasSelectors.sinResultados);

        if (elemento) {
            elemento.classList.toggle('d-none', !mostrar);
        }
    },

    deshabilitarBotones: function (deshabilitar) {
        const selectores = [
            tareasSelectors.btnGuardar,
            tareasSelectors.btnActualizar,
            tareasSelectors.btnEliminar
        ];

        selectores.forEach(selector => {
            const boton = document.querySelector(selector);

            if (boton) {
                boton.disabled = deshabilitar;
            }
        });
    },

    cerrarModalConfirmacion: function () {
        const modal = document.querySelector(tareasSelectors.modalConfirmacion);

        if (!modal) return;

        if (window.bootstrap) {
            bootstrap.Modal.getOrCreateInstance(modal).hide();
            return;
        }

        if (window.jQuery) {
            $(modal).modal('hide');
        }
    },

    configurarFechaLimiteMinima: function () {
        const inputFechaLimite = document.querySelector(tareasSelectors.fechaLimite);

        if (!inputFechaLimite) return;

        const ahora = new Date();
        const fechaLocal = new Date(ahora.getTime() - ahora.getTimezoneOffset() * 60000);

        inputFechaLimite.min = fechaLocal.toISOString().split('T')[0];
    }
};

//#endregion

//#region Obtener Datos del Formulario

const obtenerDatosFormularioTarea = () => {
    return {
        titulo:
            obtenerValor(
                tareasSelectors.titulo
            ).trim(),

        descripcion:
            obtenerValor(
                tareasSelectors.descripcion
            ).trim() || null,

        idPrioridad:
            obtenerNumero(
                tareasSelectors.prioridad
            ),

        fechaInicio:
            obtenerValor(
                tareasSelectors.fechaInicio
            ) || null,

        fechaFinalizacion:
            obtenerValor(
                tareasSelectors.fechaFinalizacion
            ) || null,

        fechaLimite:
            obtenerValor(
                tareasSelectors.fechaLimite
            ),

        idEstatus:
            obtenerNumero(
                tareasSelectors.estatus
            ),

        idUsuarioResponsable:
            obtenerNumero(
                tareasSelectors.usuarioResponsable
            )
    };
};

//#endregion

//#region Validación del Formulario

const validarFormularioTarea = () => {

    const campos = [
        {
            selector: tareasSelectors.titulo,
            mensaje: 'El título es obligatorio.'
        },
        {
            selector: tareasSelectors.prioridad,
            mensaje: 'Selecciona una prioridad.'
        },
        {
            selector: tareasSelectors.fechaLimite,
            mensaje: 'La fecha límite es obligatoria.'
        },
        {
            selector: tareasSelectors.estatus,
            mensaje: 'Selecciona un estatus.'
        },
        {
            selector: tareasSelectors.usuarioResponsable,
            mensaje: 'Selecciona un responsable.'
        }
    ];

    for (const campo of campos) {
        const elemento =
            document.querySelector(campo.selector);

        if (!elemento) {
            continue;
        }

        const valor =
            String(elemento.value ?? '').trim();

        if (!valor) {
            mostrarAlerta(
                campo.selector,
                campo.mensaje,
                'warning'
            );

            elemento.focus();

            return false;
        }
    }

    const descripcion = obtenerValor(tareasSelectors.descripcion);

    // Valida Longitud de Descripcion
    if (descripcion.length > 500) {
        mostrarAlerta(
            tareasSelectors.descripcion,
            'La descripción no puede superar los 500 caracteres.',
            'warning'
        );

        return false;
    }

    const fechaLimite = obtenerValor(tareasSelectors.fechaLimite);

    // Valida Fecha Limite no menor a Fecha Actual
    if (fechaLimite) {

        const hoy = obtenerFechaActual();

        if (fechaLimite < hoy) {
            mostrarAlerta(
                tareasSelectors.fechaLimite,
                'La fecha límite no puede ser menor a la fecha actual.',
                'warning'
            );

            return false;
        }
    }

    return true;
};

//#endregion

//#region Inicio

document.addEventListener('DOMContentLoaded', function () {
    TareasManager.init();
});

//#endregion