// src/pages/ApproverDashboard.jsx
import { useEffect, useState } from 'react';
import Navbar from '../components/Navbar';
import {
  getUsuariosPendentes,
  aprovarUsuario,
  getUsuarioById,
} from '../services/usuarioService';

export default function ApproverDashboard() {
  const [usuariosPendentes, setUsuariosPendentes] = useState([]);
  const [loading, setLoading] = useState(true);

  const carregarUsuariosPendentes = async () => {
    try {
      setLoading(true);
      const data = await getUsuariosPendentes();
      setUsuariosPendentes(data);
    } catch (error) {
      console.error(error);
      alert('Erro ao buscar usuários pendentes.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    carregarUsuariosPendentes();
  }, []);

  const aprovar = async (id) => {
    try {
      await aprovarUsuario(id);
      setUsuariosPendentes(prev => prev.filter(user => user.id !== id));
      alert('Usuário aprovado com sucesso!');
    } catch (error) {
      console.error(error);
      alert('Erro ao aprovar usuário. Verifique se você tem permissão.');
    }
  };

  return (
    <>
      <Navbar />
      <div className="p-4 max-w-4xl mx-auto">
        <h2 className="text-3xl font-bold mb-6">Painel do Aprovador</h2>

        {loading ? (
          <p>Carregando usuários pendentes...</p>
        ) : usuariosPendentes.length === 0 ? (
          <p className="text-gray-600">Nenhum usuário pendente.</p>
        ) : (
          <ul className="space-y-4">
            {usuariosPendentes.map(user => (
              <li
                key={user.id}
                className="flex justify-between items-center bg-white p-4 rounded shadow"
              >
                <div>
                  <p className="font-semibold">{user.nome}</p>
                  <p className="text-sm text-gray-500">{user.email}</p>
                </div>
                <button
                  className="bg-green-600 text-white px-4 py-2 rounded hover:bg-green-700 transition"
                  onClick={() => aprovar(user.id)}
                >
                  Aprovar
                </button>
              </li>
            ))}
          </ul>
        )}
      </div>
    </>
  );
}
