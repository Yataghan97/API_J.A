import React, { useEffect, useState } from 'react';
import api from '../services/api';

function UserDashboard() {
  const [usuario, setUsuario] = useState(null);
  const [erro, setErro] = useState(null);

  useEffect(() => {
    const token = localStorage.getItem('token');
    if (!token) {
      setErro('Usuário não autenticado. Faça login.');
      return;
    }

    api.get('/usuarios/me')
      .then(res => {
        setUsuario(res.data);
      })
      .catch(err => {
        console.error('Erro ao buscar usuário:', err);
        setErro('Erro ao buscar usuário. Veja o console.');
      });
  }, []);

  if (erro) {
    return <div className="p-4 text-red-600">{erro}</div>;
  }

  return (
    <div className="p-4">
      <h1 className="text-xl font-bold mb-4">Painel do Usuário</h1>
      {usuario ? (
        <div className="bg-white p-4 rounded shadow">
          <p><strong>Nome:</strong> {usuario.nome}</p>
          <p><strong>Email:</strong> {usuario.email}</p>
          <p><strong>Idade:</strong> {usuario.idade}</p>
          <p><strong>Aprovado:</strong> {usuario.isAprovado ? 'Sim' : 'Não'}</p>
        </div>
      ) : (
        <p>Carregando...</p>
      )}
    </div>
  );
}

export default UserDashboard;
