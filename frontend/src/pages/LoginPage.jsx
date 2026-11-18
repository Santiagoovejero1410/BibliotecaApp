import { useState } from "react";
import { Link, Navigate, useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext.jsx";
import { getError } from "../api/api.js";

export default function LoginPage() {
  const { user, login } = useAuth();
  const navigate = useNavigate();

  const [form, setForm] = useState({ email: "", password: "" });
  const [errores, setErrores] = useState({});     
  const [errorApi, setErrorApi] = useState("");   
  const [cargando, setCargando] = useState(false);


  if (user) return <Navigate to="/" replace />;

  const handleChange = (e) => setForm({ ...form, [e.target.name]: e.target.value });

  const validar = () => {
    const e = {};
    if (!form.email.trim()) e.email = "El email es obligatorio";
    else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(form.email)) e.email = "El email no es válido";
    if (!form.password) e.password = "La contraseña es obligatoria";
    return e;
  };

  const handleSubmit = async (ev) => {
    ev.preventDefault();                      
    const e = validar();
    setErrores(e);
    if (Object.keys(e).length > 0) return;        

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
    <div className="auth-page"><div className="auth-layout"><aside className="auth-art"><div><div className="auth-symbol">▤</div><div className="eyebrow">Biblioteca</div><h2>Tu próxima historia<br />empieza acá.</h2><p>Ingresá a tu cuenta para descubrir libros, gestionar tus préstamos y seguir leyendo.</p></div><div className="small">LEER · DESCUBRIR · COMPARTIR</div></aside>
      <form className="auth-card" onSubmit={handleSubmit} noValidate>
        <h1>Bienvenido de nuevo</h1>
        <p className="muted">Ingresá tus datos para continuar.</p>

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
      </form></div>
    </div>
  );
}
