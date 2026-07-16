/**
 * -----------------------------------------------------------------------------
 * Archivo: api.js
 * Proyecto: TaskManagement
 * Autor: Alejandro Arizmendi
 * Fecha: 15/07/2026
 *
 * Cliente HTTP reutilizable para consumir la API REST de TaskManagement.
 *
 * Funcionalidades:
 *  - GET
 *  - POST
 *  - PUT
 *  - DELETE
 *  - Manejo centralizado de errores
 *  - Interpretación de ResponseHelper
 *  - Conversión automática de JSON
 * 
 * Todos los métodos devuelven directamente la propiedad `data`
 * de la respuesta generada por ResponseHelper.
 * 
 * -----------------------------------------------------------------------------
 */

class ApiClient {
    constructor(baseUrl) {
        this.baseUrl = baseUrl.endsWith('/')
            ? baseUrl
            : `${baseUrl}/`;
    }

    async get(endpoint) {
        return await this.request(endpoint, {
            method: 'GET'
        });
    }

    async post(endpoint, data) {
        return await this.request(endpoint, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(data)
        });
    }

    async put(endpoint, data) {
        return await this.request(endpoint, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(data)
        });
    }

    async delete(endpoint) {
        return await this.request(endpoint, {
            method: 'DELETE'
        });
    }

    async request(endpoint, options) {
        const response = await fetch(
            `${this.baseUrl}${endpoint}`,
            options
        );

        const result = await this.parseResponse(response);

        if (!response.ok) {
            throw new ApiError(
                result?.message ?? `Error HTTP ${response.status}`,
                response.status,
                result
            );
        }

        return result;
    }

    async parseResponse(response) {
        const contentType = response.headers.get('content-type');

        if (contentType?.includes('application/json')) {
            return await response.json();
        }

        const text = await response.text();

        return text
            ? {
                success: response.ok,
                message: text,
                data: null,
                code: response.status
            }
            : {
                success: response.ok,
                message: null,
                data: null,
                code: response.status
            };
    }
}

class ApiError extends Error {
    constructor(message, status, response) {
        super(message);

        this.name = 'ApiError';
        this.status = status;
        this.response = response;
    }
}

const api = new ApiClient(window.appSettings.apiUrl);