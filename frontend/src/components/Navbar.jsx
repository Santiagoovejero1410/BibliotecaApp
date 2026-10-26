import { Link, useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext.jsx";

// Barra de navegación. Si no hay usuario logueado, no se muestra.
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
      <strong className="logo">📚 Biblioteca</strong>

      <nav className="nav-links">
        <Link to="/">Libros</Link>
        <Link to="/prestamos">Préstamos</Link>
      </nav>

      <div className="nav-user">
        <span>{user.nombre} ({user.rol})</span>
        <button className="btn btn-outline" onClick={salir}>Salir</button>
      </div>
    </header>
  );
}
