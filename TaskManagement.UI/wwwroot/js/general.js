//#region Show Alertity

const notyf = new Notyf({
    duration: 4000,
    position: {
        x: 'right',
        y: 'bottom'        
    },
    dismissible: true,
    ripple: true
});

function mostrarAlerta(selector = null, mensaje = '', tipoNotificacion = 'success') {

    if (!mensaje || typeof mensaje !== 'string') return;

    const control = selector ? $(selector) : null;

    switch (tipoNotificacion.toLowerCase()) {

        case 'success':

            notyf.success(mensaje);

            if (control && control.length > 0) {
                control.removeClass('is-invalid').addClass('is-valid');
            }

            break;

        case 'warning':

            notyf.open({
                type: 'warning',
                message: mensaje,
                background: '#f39c12'
            });

            if (control && control.length > 0) {

                control
                    .removeClass('is-valid is-invalid')
                    .addClass('is-invalid');

                control[0].focus();

                control[0].scrollIntoView({
                    behavior: 'smooth',
                    block: 'center'
                });
            }

            break;

        case 'error':

            notyf.error(mensaje);

            if (control && control.length > 0) {

                control
                    .removeClass('is-valid')
                    .addClass('is-invalid');

                control[0].focus();

                control[0].scrollIntoView({
                    behavior: 'smooth',
                    block: 'center'
                });
            }

            break;

        case 'info':

            notyf.open({
                type: 'info',
                message: mensaje,
                background: '#3498db'
            });

            break;

        default:

            notyf.open({
                type: 'custom',
                message: mensaje,
                background: '#6c757d'
            });

            break;
    }
}

function mostrarError(error, mensajePredeterminado = 'Ocurrió un error.') {

    const mensaje =
        error?.response?.message ??
        error?.message ??
        mensajePredeterminado;

    mostrarAlerta(null, mensaje, 'error');
}

//#endregion

//#region Funciones Fechas

function formatearFecha(fecha) {

    if (!fecha)
        return "";

    // Si viene como DateOnly (yyyy-MM-dd)
    if (fecha.length === 10)
        fecha += "T00:00:00";

    const date = new Date(fecha);

    if (isNaN(date))
        return "";

    return date.toLocaleDateString("es-MX", {
        day: "2-digit",
        month: "2-digit",
        year: "numeric"
    });
}

function normalizarFechaInput(fecha) {

    if (!fecha)
        return "";

    const date = new Date(fecha);

    if (isNaN(date))
        return "";

    const year = date.getFullYear();

    const month = String(date.getMonth() + 1)
        .padStart(2, "0");

    const day = String(date.getDate())
        .padStart(2, "0");

    return `${year}-${month}-${day}`;

}

function obtenerFechaActual() {

    const hoy = new Date();

    const year = hoy.getFullYear();

    const month = String(hoy.getMonth() + 1).padStart(2, "0");

    const day = String(hoy.getDate()).padStart(2, "0");

    return `${year}-${month}-${day}`;
}

//#endregion

//#region Funciones

function llenarSelect(selector, elementos, campoValor, campoTexto, textoInicial = "Seleccione una opción") {

    const select = document.querySelector(selector);

    if (!select) return;

    // Limpiar opciones
    select.innerHTML = "";

    // Opción por defecto
    const opcionInicial = document.createElement("option");
    opcionInicial.value = "";
    opcionInicial.textContent = textoInicial;

    select.appendChild(opcionInicial);

    // Agregar elementos
    elementos.forEach(item => {

        const option = document.createElement("option");

        option.value = item[campoValor];
        option.textContent = item[campoTexto];

        select.appendChild(option);
    });
}

function obtenerNumero(selector) {

    const valor = obtenerValor(selector);

    if (valor === "")
        return null;

    const numero = Number(valor);

    return Number.isNaN(numero)
        ? null
        : numero;

}

function asignarValor(selector, valor) {

    const elemento = document.querySelector(selector);

    if (!elemento) return;

    elemento.value = valor ?? "";
}

function obtenerBadgePrioridad(prioridad) {

    switch ((prioridad ?? "").toLowerCase()) {

        case "alta":
            return `<span class="badge-custom badge-prioridad-alta">
                        <i class="fas fa-arrow-up me-1"></i>
                        Alta
                    </span>`;

        case "media":
            return `<span class="badge-custom badge-prioridad-media">
                        <i class="fas fa-minus me-1"></i>
                        Media
                    </span>`;

        case "baja":
            return `<span class="badge-custom badge-prioridad-baja">
                        <i class="fas fa-arrow-down me-1"></i>
                        Baja
                    </span>`;

        default:
            return `<span class="badge bg-secondary">${escapeHtml(prioridad)}</span>`;
    }

}

function obtenerBadgeEstatus(estatus) {

    switch ((estatus ?? "").toLowerCase()) {

        case "pendiente":
            return `<span class="badge-custom badge-estatus-pendiente">
                        <i class="fas fa-clock me-1"></i>
                        Pendiente
                    </span>`;

        case "en progreso":
            return `<span class="badge-custom badge-estatus-progreso">
                        <i class="fas fa-spinner me-1"></i>
                        En progreso
                    </span>`;

        case "completada":
            return `<span class="badge-custom badge-estatus-completada">
                        <i class="fas fa-circle-check me-1"></i>
                        Completada
                    </span>`;

        case "cancelada":
            return `<span class="badge-custom badge-estatus-cancelada">
                        <i class="fas fa-ban me-1"></i>
                        Cancelada
                    </span>`;

        default:
            return `<span class="badge bg-secondary">${escapeHtml(estatus)}</span>`;
    }

}

//#endregion

//#region Funciones - Construir Query

function obtenerValor(selector) {

    const elemento = document.querySelector(selector);

    if (!elemento) return "";

    return elemento.value?.trim() ?? "";
}

function agregarParametroSiExiste(params, nombre, valor) {

    if (valor === null ||
        valor === undefined ||
        valor === "") {
        return;
    }

    params.append(nombre, valor);
}

//#endregion

//#region Funciones - Renderizar Tabla

function escapeHtml(valor) {

    if (valor === null || valor === undefined)
        return "";

    const div = document.createElement("div");

    div.textContent = valor;
    return div.innerHTML;
}

function escapeJs(valor) {

    if (valor === null || valor === undefined)
        return "";

    return String(valor)
        .replace(/\\/g, "\\\\")   // \
        .replace(/'/g, "\\'")     // '
        .replace(/"/g, '\\"')     // "
        .replace(/\r/g, "\\r")    // Retorno de carro
        .replace(/\n/g, "\\n")    // Salto de línea
        .replace(/\t/g, "\\t");   // Tabulación
}

//#endregion

//#region Agregar "Ver mas" - "Ver menos" a una tabla

function agregarPopOverVerMas(selector, maxPalabras) {
    const celdas = document.querySelectorAll(selector);
    const isDark = document.body.classList.contains("dark-mode");

    celdas.forEach((celda) => {
        const textoCompleto = celda.textContent.trim();
        const palabras = textoCompleto.split(" ");

        if (palabras.length > maxPalabras) {

            const textoTruncado = palabras.slice(0, maxPalabras).join(" ");
            const verMasLink = document.createElement("a");

            verMasLink.href = "#";
            verMasLink.textContent = " Ver más";
            verMasLink.classList.add("ver-mas-popover", "text-link"); // Aplica estilo

            if (isDark) {
                verMasLink.classList.add("dark-text");
            }

            verMasLink.setAttribute("tabindex", "0");
            verMasLink.setAttribute("data-bs-toggle", "popover");
            verMasLink.setAttribute("data-bs-trigger", "focus");
            verMasLink.setAttribute("data-bs-placement", "auto");
            verMasLink.setAttribute("data-bs-html", "true");
            verMasLink.setAttribute("data-bs-content", `<div class="popover-body-custom">${textoCompleto}</div>`);

            celda.innerHTML = `${textoTruncado}... `;
            celda.appendChild(verMasLink);
        }
    });

    // Inicializar todos los popovers
    const popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'));
    popoverTriggerList.forEach((popoverTriggerEl) => {
        new bootstrap.Popover(popoverTriggerEl);
    });
}


//#endregion

//#region Agregar Evento "Enter"

function agregarEventoEnter(selectorInput, callback) {
    $(selectorInput).on("keydown", function (e) {
        if (e.key === "Enter") {
            e.preventDefault();
            callback();
        }
    });
}

//#endregion
