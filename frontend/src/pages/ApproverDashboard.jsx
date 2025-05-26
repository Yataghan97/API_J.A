import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import api from '../services/api';
import '../pages_css/aprover.css'

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
      .catch(err => {
        setErro('Erro ao buscar informações do usuário.');
        setLoadingUsuario(false);
      });
  }, []);

  useEffect(() => {
    api.get('/usuarios/pendentes')
      .then(res => {
        setPendentes(res.data);
        setLoadingPendentes(false);
      })
      .catch(err => {
        setErro('Erro ao carregar usuários pendentes.');
        setLoadingPendentes(false);
      });
  }, []);

  const aprovarUsuario = async (id) => {
    try {
      await api.put(`/usuarios/${id}/aprovar`);
      alert('Usuário aprovado com sucesso!');
      const res = await api.get('/usuarios/pendentes');
      setPendentes(res.data);
    } catch {
      alert('Erro ao aprovar usuário.');
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

      <section className="pending-users">
        <h2 className="subtitle">Usuários Pendentes para Aprovação</h2>
        {loadingPendentes ? (
          <p>Carregando usuários pendentes...</p>
        ) : pendentes.length > 0 ? (
          pendentes.map(user => (
            <div key={user.id} className="card user-pending">
              <p><strong>Nome:</strong> {user.nome}</p>
              <p><strong>Email:</strong> {user.email}</p>
              <button onClick={() => aprovarUsuario(user.id)} className="btn btn-approve">
                Aprovar
              </button>
            </div>
          ))
        ) : (
          <p>Não há usuários pendentes.</p>
        )}
      </section>
    </div>
  );
}
