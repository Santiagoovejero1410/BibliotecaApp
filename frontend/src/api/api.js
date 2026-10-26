import axios from "axios";

// Instancia de Axios con la URL base de nuestro backend.
// Así en las páginas escribimos api.get("/libros") en vez de la URL completa.
const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL || "http://localhost:5080/api",
});

// Interceptor: antes de enviar CADA pedido, agrega el token JWT en el header Authorization.
// Así no hay que acordarse de mandarlo en cada llamada.
api.interceptors.request.use((config) => {
  const token = localStorage.getItem("token");
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Convierte un error de Axios en un texto para mostrar en pantalla.
export function getError(err) {
  if (!err.response) return "No se pudo conectar con el servidor.";
  const { status, data } = err.response;
  if (status === 401 && !data?.mensaje) return "Tu sesión venció. Volvé a iniciar sesión.";
  if (status === 403 && !data?.mensaje) return "No tenés permiso para hacer esto.";
  if (data?.mensaje) return data.mensaje;                                   // errores de nuestras reglas de negocio
  if (data?.errors) return Object.values(data.errors).flat().join(" ");     // errores de validación de los DTO
  return "Ocurrió un error inesperado.";
}

export default api;
