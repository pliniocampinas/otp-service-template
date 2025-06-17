document.addEventListener('DOMContentLoaded', () => {
    // Referências aos elementos do DOM
    const step1 = document.getElementById('step1');
    const step2 = document.getElementById('step2');
    const step3 = document.getElementById('step3');

    const emailInput = document.getElementById('emailInput');
    const sendOtpButton = document.getElementById('sendOtpButton');
    const emailError = document.getElementById('emailError');

    const displayEmail = document.getElementById('displayEmail');
    const otpInput = document.getElementById('otpInput');
    const verifyOtpButton = document.getElementById('verifyOtpButton');
    const otpError = document.getElementById('otpError');

    const userDataForm = document.getElementById('userDataForm');
    const fullNameInput = document.getElementById('fullName');
    const phoneInput = document.getElementById('phone');
    const cpfInput = document.getElementById('cpf');
    const cityInput = document.getElementById('city');
    const stateInput = document.getElementById('state');
    const streetInput = document.getElementById('street');
    const numberInput = document.getElementById('number');
    const cepInput = document.getElementById('cep');
    const complementInput = document.getElementById('complement');
    const formEmailInput = document.getElementById('formEmail'); // Hidden email field

    const dataLoadingMessage = document.getElementById('dataLoadingMessage');
    const dataErrorMessage = document.getElementById('dataErrorMessage');

    let userEmail = '';
    let jwtToken = '';

    // --- Funções Auxiliares ---

    // Mostra uma etapa e esconde as outras
    function showStep(stepToShow) {
        [step1, step2, step3].forEach(step => {
            step.classList.remove('active');
        });
        stepToShow.classList.add('active');
    }

    // Limpa mensagens de erro
    function clearErrors() {
        emailError.textContent = '';
        otpError.textContent = '';
        dataErrorMessage.textContent = '';
    }

    // --- Etapa 1: Incluir Email ---
    sendOtpButton.addEventListener('click', async () => {
        clearErrors();
        userEmail = emailInput.value.trim();

        if (!userEmail) {
            emailError.textContent = 'Por favor, insira seu email.';
            return;
        }

        if (!/^[^@]+@[^@]+\.[a-zA-Z]{2,}$/.test(userEmail)) {
            emailError.textContent = 'Por favor, insira um email válido.';
            return;
        }

        try {
            sendOtpButton.disabled = true; // Desabilita o botão para evitar múltiplos cliques
            
            await fetch('/otp/start', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ email: userEmail })
            });

            sendOtpButton.textContent = 'Enviando...';
            await new Promise(resolve => setTimeout(resolve, 2000)); // Simula delay da API

            displayEmail.textContent = userEmail; // Exibe o email na próxima etapa
            otpInput.value = ''; // Limpa o campo OTP
            showStep(step2);

        } catch (error) {
            emailError.textContent = 'Erro ao enviar OTP. Tente novamente.';
            console.error('Erro ao enviar OTP:', error);
        } finally {
            sendOtpButton.disabled = false;
            sendOtpButton.textContent = 'Enviar OTP';
        }
    });

    // --- Etapa 2: Incluir Código OTP e Receber Token JWT ---
    verifyOtpButton.addEventListener('click', async () => {
        clearErrors();
        const otpCode = otpInput.value.trim();

        if (!otpCode) {
            otpError.textContent = 'Por favor, insira o código OTP.';
            return;
        }

        if (!/^\d{6}$/.test(otpCode)) {
            otpError.textContent = 'O OTP deve ter 6 dígitos numéricos.';
            return;
        }

        // Simulação de verificação de OTP e obtenção de JWT
        try {
            // Em um ambiente real, você faria uma requisição POST para sua API de backend
            const response = await fetch('/otp/confirm', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ email: userEmail, password: otpCode })
            });

            if (!response.ok) {
                const errorData = await response.json();
                throw new Error(errorData.message || 'Falha na verificação do OTP.');
            }

            const data = await response.json();
            jwtToken = data.token; // Armazena o token JWT

            verifyOtpButton.disabled = true;
            verifyOtpButton.textContent = 'Verificando...';
            await new Promise(resolve => setTimeout(resolve, 2000)); // Simula delay da API

            // Exemplo de JWT (apenas para simulação, não use em produção)
            console.log('Token JWT recebido:', jwtToken);

            showStep(step3);
            loadUserData(); // Carrega os dados após obter o token

        } catch (error) {
            otpError.textContent = `Erro: ${error.message || 'OTP inválido ou expirado.'}`;
            console.error('Erro ao verificar OTP:', error);
        } finally {
            verifyOtpButton.disabled = false;
            verifyOtpButton.textContent = 'Verificar OTP e Obter Token';
        }
    });

    // --- Etapa 3: Carregar dados da API e preencher formulário ---
    async function loadUserData() {
        dataLoadingMessage.style.display = 'block';
        dataErrorMessage.textContent = '';
        formEmailInput.value = userEmail; // Preenche o campo de email oculto

        try {
            const response = await fetch('/user', {
                method: 'GET',
                headers: {
                    'Authorization': `Bearer ${jwtToken}`
                }
            });

            if (!response.ok) {
                throw new Error('Falha ao carregar dados do usuário.');
            }

            const userResponse = await response.json()

            // Dados fictícios para o restante do formulário
            const userData = {
                email: userResponse.email, // Usamos o email que o usuário digitou
                fullName: userResponse.fullName || 'Nome Completo Fictício',
                phone: userResponse.phone || '(XX) XXXXX-XXXX',
                cpf: 'XXX.XXX.XXX-XX', // Fictício
                city: userResponse?.address?.city || 'Cidade Fictícia',
                state: 'UF', // Fictício
                street: userResponse?.address?.street || 'Rua Fictícia',
                number: '123', // Fictício
                cep: 'XXXXX-XXX', // Fictício
                complement: 'Apto 101' // Fictício
            };

            // Preenche o formulário
            fullNameInput.value = userData.fullName;
            phoneInput.value = userData.phone;
            cpfInput.value = userData.cpf;
            cityInput.value = userData.city;
            stateInput.value = userData.state;
            streetInput.value = userData.street;
            numberInput.value = userData.number;
            cepInput.value = userData.cep;
            complementInput.value = userData.complement;

        } catch (error) {
            dataErrorMessage.textContent = `Erro ao carregar dados: ${error.message}`;
            console.error('Erro ao carregar dados do usuário:', error);
        } finally {
            dataLoadingMessage.style.display = 'none';
        }
    }
});