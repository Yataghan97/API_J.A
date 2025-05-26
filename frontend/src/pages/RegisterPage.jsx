import { useState } from 'react';
import api from '../services/api';

export default function RegisterPage() {
  const [form, setForm] = useState({ nome: '', email: '', senha: '' });

  const handleChange = (e) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  const handleRegister = async (e) => {
    e.preventDefault();
    try {
      await api.post('/usuarios', form);
      alert('Cadastro enviado para aprovação!');
      // Limpar campos após cadastro
      setForm({ nome: '', email: '', senha: '' });
    } catch {
      alert('Erro ao cadastrar.');
    }
  };

  return (
    <div className="min-h-screen">
      <form
        className="bg-white shadow-md rounded p-6 max-w-sm w-full"
        onSubmit={handleRegister}
        autoComplete="off" // Desativa preenchimento automático
      >
        <h2 className="text-xl">Cadastro</h2>
        <input
          name="nome"
          placeholder="Nome"
          className="input"
          onChange={handleChange}
          value={form.nome}
          autoComplete="off"
        />
        <input
          name="email"
          placeholder="Email"
          className="input"
          onChange={handleChange}
          value={form.email}
          type="email"
          autoComplete="off"
        />
        <input
          name="senha"
          placeholder="Senha"
          type="password"
          className="input"
          onChange={handleChange}
          value={form.senha}
          autoComplete="new-password"
        />
        <button type="submit" className="btn bg-blue-600">Cadastrar</button>
      </form>
    </div>
  );
}
