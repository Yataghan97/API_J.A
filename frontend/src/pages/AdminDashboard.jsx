import { useEffect, useState } from 'react';
import Navbar from '../components/Navbar';
import api from '../services/api';

export default function AdminDashboard() {
  const [usuarios, setUsuarios] = useState([]);

  // Carrega os usuários ao iniciar
  useEffect(() => {
    carregarUsuarios();
  }, []);

  const carregarUsuarios = async () => {
    try {
      const response = await api.get('/usuarios');
      setUsuarios(response.data);
    } catch (error) {
      alert("Erro ao carregar usuários.");
    }
  };

  const downloadUsers = async () => {
    try {
      const response = await api.get('/usuarios');
      const blob = new Blob([JSON.stringify(response.data)], { type: 'application/json' });
      const link = document.createElement('a');
      link.href = URL.createObjectURL(blob);
      link.download = 'usuarios.json';
      link.click();
    } catch (error) {
      alert("Erro ao baixar usuários.");
    }
  };

  const aprovarUsuario = async (id) => {
    try {
      await api.put(`/usuarios/{id}/aprovar`);
      alert("Usuário aprovado com sucesso!");
      carregarUsuarios(); // Recarrega a lista após aprovação
    } catch (error) {
      alert("Erro ao aprovar usuário.");
    }
  };

  return (
    <>
      <Navbar />
      <div className="p-4">
        <h2 className="text-2xl mb-4">Painel do Administrador</h2>

        <button
          className="bg-blue-600 text-white p-2 rounded mb-6"
          onClick={downloadUsers}
        >
          Baixar usuários em JSON
        </button>

        <div className="space-y-4">
          {usuarios.map((user) => (
            <div
              key={user.id}
              className="border p-4 rounded shadow bg-white"
            >
              <p><strong>Nome:</strong> {user.nome}</p>
              <p><strong>Email:</strong> {user.email}</p>
              <p><strong>Cargo:</strong> {user.role}</p>
              <p><strong>Aprovado:</strong> {user.isAprovado ? 'Sim' : 'Não'}</p>

              {!user.isAprovado && (
                <button
                  onClick={() => aprovarUsuario(user.id)}
                  className="mt-2 bg-green-600 text-white px-4 py-2 rounded"
                >
                  Aprovar
                </button>
              )}
            </div>
          ))}
        </div>
      </div>
    </>
  );
}
