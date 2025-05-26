import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import api from '../services/api';
import '../pages_css/admin.css'; // importa o CSS

export default function AdminDashboard() {
  const [admin, setAdmin] = useState(null);
  const [usuarios, setUsuarios] = useState([]);
  const navigate = useNavigate();

  const carregarUsuario = async () => {
    try {
      const response = await api.get('/usuarios/me');
      setAdmin(response.data);
    } catch (error) {
      console.error("Erro ao carregar usuário logado:", error);
      alert("Erro ao carregar dados do administrador.");
    }
  };

  const carregarUsuarios = async () => {
    try {
      const response = await api.get('/usuarios');
      setUsuarios(response.data);
    } catch (error) {
      console.error("Erro ao carregar usuários:", error);
      alert("Erro ao carregar usuários.");
    }
  };

  const aprovarUsuario = async (id) => {
    try {
      await api.put(`/usuarios/${id}/aprovar`);
      alert("Usuário aprovado com sucesso!");
      carregarUsuarios();
    } catch (error) {
      console.error("Erro ao aprovar usuário:", error);
      alert("Erro ao aprovar usuário.");
    }
  };

  const downloadUsers = async () => {
    try {
      const response = await api.get('/usuarios');
      const blob = new Blob([JSON.stringify(response.data, null, 2)], {
        type: 'application/json',
      });
      const link = document.createElement('a');
      link.href = URL.createObjectURL(blob);
      link.download = 'usuarios.json';
      link.click();
    } catch (error) {
      alert("Erro ao baixar usuários.");
    }
  };

  const handleLogout = () => {
    localStorage.removeItem('token');
    navigate('/');
  };

  useEffect(() => {
    carregarUsuario();
    carregarUsuarios();
  }, []);

  return (
    <main className="admin-main">
      <h2 className="admin-title">Painel do Administrador</h2>

      <button onClick={handleLogout} className="btn btn-logout">
        Sair
      </button>

      {admin && (
        <section className="admin-info">
          <h3>Administrador Logado</h3>
          <p><strong>Nome:</strong> {admin.nome}</p>
          <p><strong>Email:</strong> {admin.email}</p>
          <p><strong>Idade:</strong> {admin.idade}</p>
          <p><strong>Role:</strong> {admin.role}</p>
          <p><strong>Aprovado:</strong> {admin.isAprovado ? 'Sim' : 'Não'}</p>
        </section>
      )}

      <button onClick={downloadUsers} className="btn btn-download">
        Baixar usuários em JSON
      </button>

      <div className="usuarios-lista">
        {usuarios.map((user) => (
          <div key={user.id} className="usuario-card">
            <p><strong>Nome:</strong> {user.nome}</p>
            <p><strong>Email:</strong> {user.email}</p>
            <p><strong>Cargo:</strong> {user.role}</p>
            <p><strong>Aprovado:</strong> {user.isAprovado ? 'Sim' : 'Não'}</p>

            {user.isAprovado && user.aprovador && (
              <p><strong>Aprovado por:</strong> {user.aprovador}</p>
            )}

            {!user.isAprovado && (
              <button
                onClick={() => aprovarUsuario(user.id)}
                className="btn btn-approve"
              >
                Aprovar
              </button>
            )}
          </div>
        ))}
      </div>
    </main>
  );
}
