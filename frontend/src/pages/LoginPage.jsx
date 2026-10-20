import { useState } from "react";
import { Link, Navigate, useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext.jsx";
import { mensajeError } from "../api/axios.js";
import { esEmailValido } from "../utils/validators.js";
import { ErrorMsg } from "../components/Estado.jsx";

export default function LoginPage() {
  const { user, login } = useAuth();
  const navigate = useNavigate();

  // useState: guarda lo que el usuario escribe, los errores y si se está enviando
  const [form, setForm] = useState({ email: "", password: "" });
  const [errores, setErrores] = useState({});     // errores por campo (validación del front)
  const [errorApi, setErrorApi] = useState("");   // error que devuelve el backend
  const [cargando, setCargando] = useState(false);

  // Si ya hay sesión no tiene sentido ver el login
  if (user) return <Navigate to="/" replace />;

  // Un solo handler para todos los inputs: usa el "name" del input para saber qué campo actualizar
  const handleChange = (e) => setForm({ ...form, [e.target.name]: e.target.value });

  // Validación del formulario: devuelve un objeto { campo: "mensaje" }
  const validar = () => {
    const e = {};
    if (!form.email.trim()) e.email = "El email es obligatorio";
    else if (!esEmailValido(form.email)) e.email = "El email no es válido";
    if (!form.password) e.password = "La contraseña es obligatoria";
    return e;
  };

  const handleSubmit = async (ev) => {
    ev.preventDefault();                       // Evita que el navegador recargue la página
    const e = validar();
    setErrores(e);
    if (Object.keys(e).length > 0) return;     // Si hay errores, no llamamos a la API

    setCargando(true);
    setErrorApi("");
    try {
      await login(form.email, form.password);
      navigate("/");
    } catch (err) {
      setErrorApi(mensajeError(err));
    } finally {
      setCargando(false);
    }
  };

  return (
    <div className="auth-page">
      <form className="card auth-card" onSubmit={handleSubmit} noValidate>
        <h1>🗓️ TurnosApp</h1>
        <p className="subtitulo">Iniciá sesión para gestionar tus turnos</p>

        <ErrorMsg mensaje={errorApi} />

        <label>Email</label>
        <input name="email" type="email" value={form.email} onChange={handleChange} placeholder="tu@email.com" />
        {errores.email && <p className="error-texto">{errores.email}</p>}

        <label>Contraseña</label>
        <input name="password" type="password" value={form.password} onChange={handleChange} placeholder="••••••" />
        {errores.password && <p className="error-texto">{errores.password}</p>}

        <button className="btn btn-primary btn-block" disabled={cargando}>
          {cargando ? "Ingresando..." : "Ingresar"}
        </button>

        <p className="auth-link">¿No tenés cuenta? <Link to="/register">Registrate</Link></p>
        <p className="demo-info">
          Demo: admin@turnos.com / Admin123! &nbsp;|&nbsp; cliente@turnos.com / Cliente123!
        </p>
      </form>
    </div>
  );
}
