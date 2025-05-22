import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import api from '../services/api';

function LoginPage() {
  const [email, setEmail] = useState('');
  const [senha, setSenha] = useState('');
  const [erro, setErro] = useState('');
  const navigate = useNavigate();

  const handleLogin = async () => {
    try {
      const response = await api.post('/auth/login', { email, senha });
      const token = response.data.token;
      localStorage.setItem('token', token);

      // Decodifica o token para pegar o role
      const payload = JSON.parse(atob(token.split('.')[1]));
      const role = payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];

      if (role === 'Admin') navigate('/admin');
      else if (role === 'Aprovador') navigate('/approver');
      else navigate('/user');
    } catch (err) {
      setErro('Email ou senha inválidos.');
    }
  };

  return (
    <div className="min-h-screen flex flex-col items-center justify-center bg-gray-100 p-4">
      <div className="bg-white p-6 rounded shadow-md w-full max-w-sm">
        <h2 className="text-xl font-semibold mb-4">Login</h2>
        <input
          type="email"
          placeholder="Email"
          className="input mb-2 w-full"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
        />
        <input
          type="password"
          placeholder="Senha"
          className="input mb-4 w-full"
          value={senha}
          onChange={(e) => setSenha(e.target.value)}
        />
        {erro && <p className="text-red-500 text-sm mb-2">{erro}</p>}
        <button onClick={handleLogin} className="btn w-full bg-blue-600 text-white">
          Entrar
        </button>
      </div>
    </div>
  );
}

export default LoginPage;
