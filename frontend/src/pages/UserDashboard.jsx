import React, { useEffect, useState } from 'react';
import api from '../services/api';

function UserDashboard() {
  const [usuario, setUsuario] = useState(null);

  useEffect(() => {
    const token = localStorage.getItem('token');
    const payload = JSON.parse(atob(token.split('.')[1]));
    const userId = payload.UsuarioId;

    api.get(`/usuarios/${userId}`)
      .then(res => setUsuario(res.data))
      .catch(err => console.error(err));
  }, []);

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
