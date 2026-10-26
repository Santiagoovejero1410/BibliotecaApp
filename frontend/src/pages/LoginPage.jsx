import { useState } from "react";
import { Link, Navigate, useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext.jsx";
import { getError } from "../api/api.js";

export default function LoginPage() {
  const { user, login } = useAuth();
  const navigate = useNavigate();

  // useState: guarda lo que se escribe, los errores y si se está enviando
  const [form, setForm] = useState({ email: "", password: "" });
  const [errores, setErrores] = useState({});     // errores de validación por campo
  const [errorApi, setErrorApi] = useState("");   // error que devuelve el backend
  const [cargando, setCargando] = useState(false);

  // Si ya hay sesión, no tiene sentido ver el login
  if (user) return <Navigate to="/" replace />;

  // Un solo handler para todos los inputs: usa el "name" del input para saber qué campo actualizar
  const handleChange = (e) => setForm({ ...form, [e.target.name]: e.target.value });

  // Validación: devuelve un objeto { campo: "mensaje de error" }
  const validar = () => {
    const e = {};
    if (!form.email.trim()) e.email = "El email es obligatorio";
    else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(form.email)) e.email = "El email no es válido";
    if (!form.password) e.password = "La contraseña es obligatoria";
    return e;
  };

  const handleSubmit = async (ev) => {
    ev.preventDefault();                          // evita que el navegador recargue la página
    const e = validar();
    setErrores(e);
    if (Object.keys(e).length > 0) return;        // si hay errores, no llamamos a la API

    setCargando(true);
    setErrorApi("");
    try {
      await login(form.email, form.password);
      navigate("/");
    } catch (err) {
      setErrorApi(getError(err));
    } finally {
      setCargando(false);
    }
  };

  return (
    <div className="auth-page">
      <form className="card auth-card" onSubmit={handleSubmit} noValidate>
        <h1>📚 Biblioteca</h1>
        <p className="muted">Iniciá sesión</p>

        {errorApi && <p className="alerta">{errorApi}</p>}

        <label>Email</label>
        <input name="email" type="email" value={form.email} onChange={handleChange} />
        {errores.email && <p className="error-texto">{errores.email}</p>}

        <label>Contraseña</label>
        <input name="password" type="password" value={form.password} onChange={handleChange} />
        {errores.password && <p className="error-texto">{errores.password}</p>}

        <button className="btn btn-primary btn-block" disabled={cargando}>
          {cargando ? "Ingresando..." : "Ingresar"}
        </button>

        <p className="muted">¿No tenés cuenta? <Link to="/register">Registrate</Link></p>
        <p className="muted small">Demo: admin@biblioteca.com / Admin123! — socio@biblioteca.com / Socio123!</p>
      </form>
    </div>
  );
}
