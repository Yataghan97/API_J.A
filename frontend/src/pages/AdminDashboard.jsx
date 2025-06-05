import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import api from '../services/api';
import '../pages_css/admin.css';

export default function AdminDashboard() {
  const [admin, setAdmin] = useState(null);
  const [usuarios, setUsuarios] = useState([]);
  const [dominios, setDominios] = useState([]);
  const [dominioSelecionado, setDominioSelecionado] = useState('');
  const [modalAberto, setModalAberto] = useState(false);
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
      const usuariosOrdenados = response.data.sort((a, b) => {
        if (a.isAprovado === b.isAprovado) return 0;
        return a.isAprovado ? 1 : -1;
      });
      setUsuarios(usuariosOrdenados);
    } catch (error) {
      console.error("Erro ao carregar usuários:", error);
      alert("Erro ao carregar usuários.");
    }
  };
    const negarUsuario = async (id) => {
    try {
      await api.put(`/usuarios/${id}/negar`);
      alert('Usuário negado com sucesso!');
      carregarUsuarios();
    } catch {
      alert('Erro ao negar usuário.');
    }
  };

  const carregarDominios = async () => {
    try {
      const response = await api.get('/usuarios/dominios');
      setDominios(response.data);
      if (response.data.length > 0) {
        setDominioSelecionado(response.data[0]);
      }
    } catch (error) {
      console.error("Erro ao carregar domínios:", error);
      alert("Erro ao carregar domínios.");
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
      const blob = new Blob([JSON.stringify(response.data, null, 2)], { type: 'application/json' });
      const link = document.createElement('a');
      link.href = URL.createObjectURL(blob);
      link.download = 'usuarios.json';
      link.click();
      URL.revokeObjectURL(link.href);
    } catch (error) {
      alert("Erro ao baixar usuários.");
    }
  };

  const downloadUsuariosPorDominio = async () => {
    if (!dominioSelecionado) {
      alert("Por favor, selecione um domínio.");
      return;
    }

    try {
      const response = await api.get(`/usuarios/dominio/${dominioSelecionado}`);
      const blob = new Blob([JSON.stringify(response.data, null, 2)], { type: 'application/json' });
      const link = document.createElement('a');
      link.href = URL.createObjectURL(blob);
      link.download = `usuarios_dominio_${dominioSelecionado}.json`;
      link.click();
      URL.revokeObjectURL(link.href);
      setModalAberto(false); // Fecha o modal depois do download
    } catch (error) {
      alert("Erro ao baixar os usuários do domínio selecionado.");
    }
  };

  const handleLogout = () => {
    localStorage.removeItem('token');
    navigate('/');
  };

  useEffect(() => {
    carregarUsuario();
    carregarUsuarios();
    carregarDominios();
  }, []);

  return (
    <main className="admin-main">
      <h2 className="admin-title">Painel do Administrador</h2>

      <div className="logout-container">
      <button onClick={handleLogout} className="btn btn-logout">
        Sair
      </button>
      </div>


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

      <div className="btn-download-group">
        <button onClick={downloadUsers} className="btn btn-download btn-small">
          Baixar usuários Geral
        </button>

        <button onClick={() => setModalAberto(true)} className="btn btn-download btn-small">
          Baixar usuários por domínio
        </button>
      </div>

      {modalAberto && (
        <div className="modal-overlay">
          <div className="modal-content">
            <h3>Selecione o domínio</h3>
            <select
              value={dominioSelecionado}
              onChange={e => setDominioSelecionado(e.target.value)}
            >
              {dominios.map(d => (
                <option key={d} value={d}>{d}</option>
              ))}
            </select>

            <div className="modal-buttons">
              <button onClick={() => setModalAberto(false)} className="btn btn-cancel">
                Cancelar
              </button>
              <button onClick={downloadUsuariosPorDominio} className="btn btn-confirm">
                Baixar
              </button>
            </div>
          </div>
        </div>
      )}

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
            <div className="btn-action-group">
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
          )}                      
          </div>
        ))}
      </div>
    </main>
  );
}
