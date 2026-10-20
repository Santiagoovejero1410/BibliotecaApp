import axios from "axios";

// Instancia ÚNICA de Axios configurada para hablar con nuestro backend.
const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL || "http://localhost:5080/api",
});

// INTERCEPTOR DE REQUEST: antes de enviar CADA pedido, agrega el token JWT en el header.
// Así no hay que acordarse de mandarlo en cada llamada.
api.interceptors.request.use((config) => {
  const token = localStorage.getItem("token");
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// INTERCEPTOR DE RESPONSE: si el backend responde 401 (token vencido/inválido) y había sesión,
// borramos la sesión y mandamos al login.
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401 && localStorage.getItem("token")) {
      localStorage.removeItem("token");
      localStorage.removeItem("user");
      window.location.href = "/login";
    }
    return Promise.reject(error);
  }
);

// Convierte cualquier error de Axios en un texto legible para mostrar en pantalla.
export function mensajeError(err) {
  if (!err.response) return "No se pudo conectar con el servidor.";
  const data = err.response.data;
  // Errores de validación de ASP.NET: { errors: { Campo: ["mensaje"] } }
  if (data?.errors) return Object.values(data.errors).flat().join(" ");
  // Errores de nuestro middleware (ProblemDetails): { detail: "mensaje" }
  return data?.detail || data?.title || "Ocurrió un error inesperado.";
}

export default api;
