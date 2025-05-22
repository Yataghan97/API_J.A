import { Link } from 'react-router-dom';

export default function Navbar() {
  return (
    <nav className="bg-blue-600 text-white p-4 flex justify-between">
      <h1 className="text-xl font-bold">Sistema de Acesso</h1>
      <Link to="/" className="hover:underline">Sair</Link>
    </nav>
  );
}
