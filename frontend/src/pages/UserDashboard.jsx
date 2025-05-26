import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import api from '../services/api';
import '../pages_css/user.css'

function UsuarioDashboard() {
  const [usuario, setUsuario] = useState(null);
  const [erro, setErro] = useState(null);
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();

  useEffect(() => {
    const token = localStorage.getItem('token');
    if (!token) {
      setErro('Usuário não autenticado. Faça login.');
      setLoading(false);
      return;
    }

    api.get('/usuarios/me')
      .then(res => {
        setUsuario(res.data);
        setLoading(false);
      })
      .catch(err => {
        console.error('Erro ao buscar usuário:', err);
        setErro('Erro ao buscar usuário. Veja o console.');
        setLoading(false);
      });
  }, []);

  const handleLogout = () => {
    localStorage.removeItem('token');
    navigate('/');
  };

  if (loading) return <p className="loading">Carregando...</p>;

  if (erro) return <div className="erro">{erro}</div>;

  return (
    <div className="usuario-container">
      <div className="usuario-header">
        <h1 className="usuario-titulo">Painel do Usuário</h1>
        <button onClick={handleLogout} className="botao-sair">Sair</button>
      </div>
      {usuario && (
        <div className="usuario-cartao">
          <p><strong>Nome:</strong> {usuario.nome}</p>
          <p><strong>Email:</strong> {usuario.email}</p>
          <p><strong>Idade:</strong> {usuario.idade}</p>
          <p><strong>Aprovado:</strong> {usuario.isAprovado ? 'Sim' : 'Não'}</p>
        </div>
      )}
    </div>
  );
}

export default UsuarioDashboard;
