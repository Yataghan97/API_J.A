import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import api from '../services/api';

export default function RegisterPage() {
  const [form, setForm] = useState({
    nome: '',
    email: '',
    senha: '',
    confirmarSenha: '',
    role: 'User', // valor padrão
  });
  const [erro, setErro] = useState('');
  const navigate = useNavigate();

  const handleChange = (e) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  const handleRegister = async (e) => {
    e.preventDefault();

    if (form.senha !== form.confirmarSenha) {
      setErro('As senhas não coincidem.');
      return;
    }

    try {
      const { nome, email, senha, role } = form;
      await api.post('/usuarios', { nome, email, senha, role });
      alert('Cadastro enviado para aprovação!');
      setForm({
        nome: '',
        email: '',
        senha: '',
        confirmarSenha: '',
        role: 'User',
      });
      setErro('');
      navigate('/');
    } catch {
      setErro('Erro ao cadastrar. Verifique os dados e tente novamente.');
    }
  };

  const handleLogin = () => {
    navigate('/');
  };

  return (
    <div className="min-h-screen flex justify-center items-center bg-gray-100 p-4">
      <form
        className="bg-white shadow-md rounded p-6 max-w-sm w-full"
        onSubmit={handleRegister}
        autoComplete="off"
      >
        <h2 className="text-xl mb-4">Cadastro</h2>

        <input
          name="nome"
          placeholder="Nome"
          className="input mb-2 w-full"
          onChange={handleChange}
          value={form.nome}
        />
        <input
          name="email"
          placeholder="Email"
          className="input mb-2 w-full"
          onChange={handleChange}
          value={form.email}
          type="email"
        />
        <input
          name="senha"
          placeholder="Senha"
          type="password"
          className="input mb-2 w-full"
          onChange={handleChange}
          value={form.senha}
          autoComplete="new-password"
        />
        <input
          name="confirmarSenha"
          placeholder="Confirmar Senha"
          type="password"
          className="input mb-2 w-full"
          onChange={handleChange}
          value={form.confirmarSenha}
          autoComplete="new-password"
        />

        <select
          name="role"
          className="input mb-4 w-full"
          onChange={handleChange}
          value={form.role}
        >
          <option value="User">Usuário Padrão</option>
          <option value="Aprovador">Aprovador</option>
        </select>

        {erro && <p className="text-red-500 text-sm mb-2">{erro}</p>}

        <button type="submit" className="btn w-full bg-blue-600 text-white mb-2">
          Cadastrar
        </button>

        <button
          type="button"
          onClick={handleLogin}
          className="btn w-full bg-gray-300 text-black"
        >
          Voltar
        </button>
      </form>
    </div>
  );
}
