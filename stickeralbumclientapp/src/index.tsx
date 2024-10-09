import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { initializeIcons } from '@fluentui/font-icons-mdl2';
import App from './components/app/App.tsx'

import './index.css'

initializeIcons();

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <App />
  </StrictMode>,
)
