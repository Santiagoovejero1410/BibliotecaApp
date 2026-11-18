import { NavLink, useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext.jsx";

export default function Navbar() {
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  if (!user) return null;

  const salir = () => {
    logout();
    navigate("/login");
  };

  return (
    <header className="navbar">
      <div className="logo">
        <span className="logo-mark">▤</span>
        <span>Biblioteca<small>Leer · descubrir · compartir</small></span>
      </div>

      <nav className="nav-links">
        <NavLink to="/" end>Libros</NavLink>
        <NavLink to="/prestamos">Préstamos</NavLink>
      </nav>

      <div className="nav-user">
        <span className="user-chip">
          <span className="avatar">{user.nombre?.charAt(0)?.toUpperCase() || "U"}</span>
          <span>{user.nombre} ({user.rol})</span>
        </span>
        <button className="btn btn-outline" onClick={salir}>Salir</button>
      </div>
    </header>
  );
}
