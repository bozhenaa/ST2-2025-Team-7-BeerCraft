document.addEventListener('DOMContentLoaded', function () {
    const bubble = document.getElementById('chatBubble');
    const window = document.getElementById('chatWindow');
    const closeBtn = document.getElementById('closeChatBtn');
    const chatForm = document.getElementById('chatForm');
    const chatInput = document.getElementById('chatInput');
    const messagesArea = document.getElementById('chatMessages');

    if (!bubble || !window || !closeBtn || !chatForm || !chatInput || !messagesArea) {
        console.error("Chatbot elements not found!");
        return;
    }

    bubble.addEventListener('click', () => {
        window.style.display = 'flex';
        bubble.style.display = 'none';
    });

    closeBtn.addEventListener('click', () => {
        window.style.display = 'none';
        bubble.style.display = 'flex';
    });

    chatForm.addEventListener('submit', async function (e) {
        e.preventDefault();
        const prompt = chatInput.value.trim();
        if (prompt === "") {
            return;
        }

        addMessage(prompt, 'user');
        chatInput.value = '';

        showTypingIndicator(true);

        try {
            const response = await fetch('/AI/GetChatResponse', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ prompt: prompt })
            });

            showTypingIndicator(false);

            if (response.ok) {
                const data = await response.json();
                addMessage(data.response, 'ai');
            } else {
                const error = await response.json();
                addMessage(`Error: ${error.error || response.statusText}`, 'ai');
            }

        } catch (error) {
            showTypingIndicator(false);
            addMessage(`Failed to connect. Is Ollama running? Error: ${error.message}`, 'ai');
        }
    });

    function addMessage(text, sender) {
        const messageDiv = document.createElement('div');
        messageDiv.classList.add('chat-message');
        messageDiv.classList.add(sender);
        messageDiv.innerHTML = `<span>${text.replace(/\n/g, '<br>')}</span>`; 
        messagesArea.appendChild(messageDiv);
        scrollToBottom();
    }

    function showTypingIndicator(show) {
        let typingIndicator = document.getElementById('typingIndicator');
        if (show) {
            if (!typingIndicator) {
                typingIndicator = document.createElement('div');
                typingIndicator.id = 'typingIndicator';
                typingIndicator.classList.add('chat-message', 'typing');
                typingIndicator.innerHTML = '<span>Typing...</span>';
                messagesArea.appendChild(typingIndicator);
                scrollToBottom();
            }
        } else {
            if (typingIndicator) {
                typingIndicator.remove();
            }
        }
    }

    function scrollToBottom() {
        messagesArea.scrollTop = messagesArea.scrollHeight;
    }
});