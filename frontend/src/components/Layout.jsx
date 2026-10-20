import { NavLink, Outlet, useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext.jsx";

// Estructura común de las páginas privadas: barra de navegación arriba + el contenido de la ruta (Outlet).
export default function Layout() {
  const { user, isAdmin, logout } = useAuth();
  const navigate = useNavigate();

  const salir = () => {
    logout();
    navigate("/login");
  };

  return (
    <div className="app-shell">
      <header className="navbar">
        <div className="logo">🗓️ TurnosApp</div>

        {/* NavLink agrega la clase "active" al link de la página actual */}
        <nav className="nav-links">
          <NavLink to="/" end>Agenda</NavLink>
          <NavLink to="/turnos">Mis turnos</NavLink>
          <NavLink to="/turnos/nuevo">Nuevo turno</NavLink>
          {/* Este link solo se renderiza si el usuario es Admin */}
          {isAdmin && <NavLink to="/admin/servicios">Servicios</NavLink>}
        </nav>

        <div className="nav-user">
          <span className="avatar">{user.nombre.charAt(0).toUpperCase()}</span>
          <span className="nav-user-info">
            {user.nombre} <small>({user.rol})</small>
          </span>
          <button className="btn btn-outline btn-small" onClick={salir}>Salir</button>
        </div>
      </header>

      <main className="contenido">
        <Outlet />
      </main>
    </div>
  );
}
