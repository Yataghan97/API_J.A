import React, { useState, useRef } from 'react';
import { useNavigate } from 'react-router-dom';
import api from '../services/api';
import '../pages_css/login.css';

function LoginPage() {
  const [email, setEmail] = useState('');
  const [senha, setSenha] = useState('');
  const [erro, setErro] = useState('');
  const navigate = useNavigate();

  const senhaRef = useRef(null);

  const handleLogin = async () => {
    try {
      const response = await api.post('/auth/login', { email, senha });
      const token = response.data.token;
      localStorage.setItem('token', token);

      const payload = JSON.parse(atob(token.split('.')[1]));
      const role = payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];

      if (role === 'Admin') navigate('/admin');
      else if (role === 'Aprovador') navigate('/approver');
      else navigate('/user');
    } catch (err) {
      if (err.response && err.response.status === 401) {
        setErro(err.response.data); // Mensagem personalizada da API
      } else {
        setErro('Erro ao tentar fazer login. Tente novamente mais tarde.');
      }
    }
  };

  const handleCriarConta = () => {
    navigate('/register');
  };

  return (
    <div className="min-h-screen flex flex-col items-center justify-center bg-gray-100 p-4">
      <form
        className="bg-white p-6 rounded shadow-md w-full max-w-sm"
        autoComplete="off"
        onSubmit={(e) => e.preventDefault()}
      >
        <h2 className="text-xl font-semibold mb-4">Login</h2>

        {/* Campo fake oculto para enganar autocomplete */}
        <input type="text" name="fakeuser" style={{ display: 'none' }} />
        <input type="password" name="fakepass" style={{ display: 'none' }} />

        <input
          type="email"
          name="login_email_field"
          autoComplete="new-email"
          placeholder="Email"
          className="input mb-2 w-full"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
        />
        <input
          type="password"
          name="login_password_field"
          autoComplete="new-password"
          placeholder="Senha"
          className="input mb-4 w-full"
          value={senha}
          onChange={(e) => setSenha(e.target.value)}
          ref={senhaRef}
        />
        {erro && <p className="text-red-500 text-sm mb-2">{erro}</p>}

        <button
          onClick={handleLogin}
          className="btn w-full bg-blue-600 text-white mb-2"
        >
          Entrar
        </button>

        <button
          onClick={handleCriarConta}
          className="btn w-full bg-gray-300 text-black"
        >
          Criar Conta
        </button>
      </form>
    </div>
  );
}

export default LoginPage;
