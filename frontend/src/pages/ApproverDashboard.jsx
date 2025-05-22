import React, { useEffect, useState } from 'react';
import api from '../services/api';

function ApproverDashboard() {
  const [usuarios, setUsuarios] = useState([]);

  useEffect(() => {
    api.get('/usuarios')
      .then(res => setUsuarios(res.data))
      .catch(err => console.error(err));
  }, []);

  const aprovarUsuario = async (id) => {
    try {
      await api.put(`/usuarios/${id}/aprovar`);
      setUsuarios(prev => prev.map(u => u.id === id ? { ...u, isAprovado: true } : u));
    } catch (err) {
      alert('Erro ao aprovar usuário.');
    }
  };

  return (
    <div className="p-4">
      <h1 className="text-xl font-bold mb-4">Painel do Aprovador</h1>
      <ul>
        {usuarios.filter(u => !u.isAprovado).map(user => (
          <li key={user.id} className="border p-2 my-2 rounded bg-white flex justify-between items-center">
            <div>
              <strong>{user.nome}</strong> - {user.email}
            </div>
            <button onClick={() => aprovarUsuario(user.id)} className="bg-blue-600 text-white px-3 py-1 rounded">
              Aprovar
            </button>
          </li>
        ))}
      </ul>
    </div>
  );
}

export default ApproverDashboard;
