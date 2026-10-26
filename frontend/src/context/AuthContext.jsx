import { createContext, useContext, useState } from "react";
import api from "../api/api.js";

// Context API = estado GLOBAL. Cualquier componente puede saber quién está logueado sin pasar props.
const AuthContext = createContext(null);

export function AuthProvider({ children }) {
  // Si ya había una sesión guardada en localStorage, la recuperamos (así no se pierde al recargar la página)
  const [user, setUser] = useState(() => {
    try {
      return JSON.parse(localStorage.getItem("user"));
    } catch {
      return null;
    }
  });

  // Guarda el token y los datos del usuario
  const guardarSesion = (data) => {
    const usuario = { id: data.id, nombre: data.nombre, email: data.email, rol: data.rol };
    localStorage.setItem("token", data.token);
    localStorage.setItem("user", JSON.stringify(usuario));
    setUser(usuario);
  };

  const login = async (email, password) => {
    const { data } = await api.post("/auth/login", { email, password });
    guardarSesion(data);
  };

  const register = async (nombre, email, password) => {
    const { data } = await api.post("/auth/register", { nombre, email, password });
    guardarSesion(data);
  };

  const logout = () => {
    localStorage.removeItem("token");
    localStorage.removeItem("user");
    setUser(null);
  };

  // Todo lo que queremos compartir con el resto de la app
  const value = { user, isAdmin: user?.rol === "Admin", login, register, logout };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

// Hook propio para usar el contexto fácil: const { user, logout } = useAuth();
export function useAuth() {
  return useContext(AuthContext);
}
