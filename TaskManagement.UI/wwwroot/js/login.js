//#region selectors

const selectors = {    
    email: '#txtEmail',
    password: '#txtPassword'    
};

//#endregion

//#region Login Manager

const LoginManager = {
    init: function () {
        const btnLogin = document.querySelector('#btnLogin');        

        if (btnLogin) btnLogin.addEventListener('click', this.loginUsuario);        
    },

    loginUsuario: async function (e) {        

        if (!validarFormulario()) return;

        const data = obtenerDatosFormulario();        

        try {

            $('#loading').show();

            const response = await api.post(
                'Login/Login',
                data
            );

            if (response.success) {
                window.location.href =
                    response.data?.redirectUrl ??
                    response.redirectUrl ??
                    '/Tareas/Index';

                return;
            }

            mostrarAlerta(
                null,
                response.message ?? 'No fue posible iniciar sesión.',
                'warning'
            );
        } catch (error) {

            console.error('Error al iniciar sesión:', error);

            if (error instanceof ApiError) {
                if (error.status === 401) {
                    mostrarAlerta(
                        null,
                        error.response?.message ??
                        'Credenciales inválidas.',
                        'warning'
                    );

                    return;
                }

                if (error.status === 400) {
                    mostrarAlerta(
                        null,
                        error.response?.message ??
                        'Revisa los datos proporcionados.',
                        'warning'
                    );

                    return;
                }

                mostrarAlerta(
                    null,
                    error.response?.message ??
                    'Ocurrió un error al procesar la solicitud.',
                    'error'
                );

                return;
            }

            mostrarAlerta(
                null,
                'No fue posible conectarse con la API.',
                'error'
            );
        } finally {
            setTimeout(() => {
                $('#loading').hide();
            }, 200);
        }
    }   
};

//#endregion

//#region Funciones

const obtenerDatosFormulario = () => {
    return {
        email: $(selectors.email).val(),
        password: $(selectors.password).val()        
    };
};

//#endregion

//#region Validaciones

function validarFormulario(){
    let campos = [
        { id: selectors.email, mensaje: 'Email es requerido', validar: validarEmail, mensajeInvalido: 'Ingresa un email con un formato correcto' },
        { id: selectors.password, mensaje: 'Password es requerido' }
    ];

    for (const campo of campos) {
        const $input = $(campo.id);
        const valor = $input.val().trim();

        if (valor === '') {
            mostrarAlerta(campo.id, campo.mensaje, 'warning')
            return false;
        }

        if (campo.validar && !campo.validar(valor)) {
            mostrarAlerta(campo.id, campo.mensajeInvalido, 'warning')
            return false;
        }
    }
    return true;
} 

//#endregion

//#region init

function inicializarPagina() {    

    agregarEventoEnter(selectors.password, LoginManager.loginUsuario);

    document.querySelectorAll(".toggle-password").forEach(icon => {

        icon.addEventListener("click", () => {

            const inputId = icon.getAttribute("data-target");
            const input = document.getElementById(inputId);

            const isPassword = input.type === "password";

            input.type = isPassword ? "text" : "password";

            icon.classList.toggle("fa-eye");
            icon.classList.toggle("fa-eye-slash");
        });
    });
}

// Inicializa Login Manager

document.addEventListener('DOMContentLoaded', function () {
    LoginManager.init();
    inicializarPagina();
    loadEventos();
});

//#endregion