import axios from "axios";

const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL || "http://localhost:5080/api",
});

api.interceptors.request.use((config) => {
  const token = localStorage.getItem("token");
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

export function getError(err) {
  if (!err.response) return "No se pudo conectar con el servidor.";
  const { status, data } = err.response;
  if (status === 401 && !data?.mensaje) return "Tu sesión venció. Volvé a iniciar sesión.";
  if (status === 403 && !data?.mensaje) return "No tenés permiso para hacer esto.";
  if (data?.mensaje) return data.mensaje;                                  
  if (data?.errors) return Object.values(data.errors).flat().join(" ");     
  return "Ocurrió un error inesperado.";
}

export default api;
