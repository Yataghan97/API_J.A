import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import api from '../services/api';
import '../pages_css/aprover.css';

export default function AprovadorDashboard() {
  const navigate = useNavigate();

  const [usuario, setUsuario] = useState(null);
  const [pendentes, setPendentes] = useState([]);
  const [erro, setErro] = useState(null);
  const [loadingUsuario, setLoadingUsuario] = useState(true);
  const [loadingPendentes, setLoadingPendentes] = useState(true);

  const sair = () => {
    localStorage.removeItem('token');
    navigate('/');
  };

  useEffect(() => {
    api.get('/usuarios/me')
      .then(res => {
        setUsuario(res.data);
        setLoadingUsuario(false);
      })
      .catch(() => {
        setErro('Erro ao buscar informações do usuário.');
        setLoadingUsuario(false);
      });
  }, []);

  useEffect(() => {
    carregarPendentes();
  }, []);

  const carregarPendentes = async () => {
    setLoadingPendentes(true);
    try {
      const res = await api.get('/usuarios/pendentes');
      setPendentes(res.data);
    } catch {
      setErro('Erro ao carregar usuários pendentes.');
    }
    setLoadingPendentes(false);
  };

  const aprovarUsuario = async (id) => {
    try {
      await api.put(`/usuarios/${id}/aprovar`);
      alert('Usuário aprovado com sucesso!');
      carregarPendentes();
    } catch {
      alert('Erro ao aprovar usuário.');
    }
  };

  const negarUsuario = async (id) => {
    try {
      await api.put(`/usuarios/${id}/negar`);
      alert('Usuário negado com sucesso!');
      carregarPendentes();
    } catch {
      alert('Erro ao negar usuário.');
    }
  };

  const downloadUsuariosMesmoDominio = async () => {
    try {
      const response = await api.get('/usuarios/usuarios_dominio');
      const blob = new Blob([JSON.stringify(response.data, null, 2)], {
        type: 'application/json',
      });
      const link = document.createElement('a');
      link.href = URL.createObjectURL(blob);
      link.download = 'usuarios_mesmo_dominio.json';
      link.click();
    } catch {
      alert("Erro ao baixar os usuários do mesmo domínio.");
    }
  };

  if (erro) {
    return <div className="error">{erro}</div>;
  }

  return (
    <div className="container">
      <button onClick={sair} className="btn btn-logout">Sair</button>

      <h1 className="title">Painel do Aprovador</h1>

      <section className="user-info">
        <h2 className="subtitle">Seus Dados</h2>
        {loadingUsuario ? (
          <p>Carregando informações do usuário...</p>
        ) : usuario ? (
          <div className="card">
            <p><strong>Nome:</strong> {usuario.nome}</p>
            <p><strong>Email:</strong> {usuario.email}</p>
            <p><strong>Idade:</strong> {usuario.idade}</p>
            <p><strong>Aprovado:</strong> {usuario.isAprovado ? 'Sim' : 'Não'}</p>
            <p><strong>Cargo:</strong> {usuario.role}</p>
          </div>
        ) : (
          <p>Usuário não encontrado.</p>
        )}
      </section>
      
      <button onClick={downloadUsuariosMesmoDominio} className="btn btn-download">
        Baixar usuários do mesmo domínio
      </button>

      <section className="pending-users">
        <h2 className="subtitle">Usuários Pendentes para Aprovação</h2>
        {loadingPendentes ? (
          <p>Carregando usuários pendentes...</p>
        ) : pendentes.length > 0 ? (
          pendentes.map(user => (
            <div key={user.id} className="usuario-card">
              <p><strong>Nome:</strong> {user.nome}</p>
              <p><strong>Email:</strong> {user.email}</p>
              {/* 'role' e 'isAprovado' não existem nessa lista */}
              <div className="btn-group">
                <button 
                  onClick={() => aprovarUsuario(user.id)} 
                  className="btn btn-approve btn-small"
                >
                  Aprovar
                </button>
                <button 
                  onClick={() => negarUsuario(user.id)} 
                  className="btn btn-deny btn-small"
                >
                  Negar
                </button>
              </div>
            </div>
          ))
        ) : (
          <p>Não há usuários pendentes.</p>
        )}
      </section>
    </div>
  );
}
