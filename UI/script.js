document.addEventListener('DOMContentLoaded', () => {

    // ===== STATE MANAGEMENT =====
    const state = {
        currentUser: null, // null, { type: 'customer' }, { type: 'vendor' }
        books: [
            { id: 1, title: 'The Midnight Library', author: 'Matt Haig', pricePDF: 12.99, pricePhysical: 18.99, cover: 'https://i.gr-assets.com/images/S/compressed.photo.goodreads.com/books/1602190253l/52578297.jpg', description: 'A dazzling novel about all the choices that go into a life well lived.' },
            { id: 2, title: 'Project Hail Mary', author: 'Andy Weir', pricePDF: 14.99, pricePhysical: 22.50, cover: 'https://i.gr-assets.com/images/S/compressed.photo.goodreads.com/books/1597695842l/54470402._SY475_.jpg', description: 'A lone astronaut must save the earth from disaster in this incredible new science-based thriller.' },
            { id: 3, title: 'Klara and the Sun', author: 'Kazuo Ishiguro', pricePDF: 11.50, pricePhysical: 17.00, cover: 'https://i.gr-assets.com/images/S/compressed.photo.goodreads.com/books/1603206535l/54120408.jpg', description: 'A magnificent new novel from the Nobel laureate in Literature.' },
            { id: 4, title: 'The Four Winds', author: 'Kristin Hannah', pricePDF: 13.00, pricePhysical: 20.00, cover: 'https://i.gr-assets.com/images/S/compressed.photo.goodreads.com/books/1601242373l/53138081.jpg', description: 'An epic novel of love and heroism and hope, set during the Great Depression.' },
        ],
        cart: [], // [{ bookId: 1, type: 'pdf' }]
    };

    // ===== SELECTORS =====
    const pages = document.querySelectorAll('.page');
    const navLinks = document.querySelectorAll('.nav-link');
    const loginForm = document.getElementById('login-form');
    const logoutBtn = document.getElementById('logout-btn');
    const bookGrid = document.getElementById('book-grid');
    const bookDetailPage = document.getElementById('book-detail-page');
    const cartCount = document.getElementById('cart-count');
    const cartItemsContainer = document.getElementById('cart-items');
    const cartTotalEl = document.getElementById('cart-total');
    const dashboardPage = document.getElementById('dashboard-page');
    const ereaderModal = document.getElementById('ereader-modal');

    // ===== RENDER FUNCTIONS =====
    const renderBookGrid = () => {
        bookGrid.innerHTML = '';
        state.books.forEach(book => {
            const bookCard = document.createElement('div');
            bookCard.className = 'book-card';
            bookCard.dataset.id = book.id;
            bookCard.innerHTML = `
                <img src="${book.cover}" alt="${book.title}" class="book-card-img">
                <div class="book-card-content">
                    <h3 class="book-card-title">${book.title}</h3>
                    <p class="book-card-author">${book.author}</p>
                    <p class="book-card-price">$${book.pricePDF.toFixed(2)} (PDF)</p>
                </div>
            `;
            bookCard.addEventListener('click', () => {
                renderBookDetail(book.id);
                showPage('book-detail-page');
            });
            bookGrid.appendChild(bookCard);
        });
    };

    const renderBookDetail = (bookId) => {
        const book = state.books.find(b => b.id === bookId);
        if (!book) return;

        bookDetailPage.innerHTML = `
            <div class="book-detail-container">
                <div class="book-detail-cover">
                    <img src="${book.cover}" alt="${book.title}">
                </div>
                <div class="book-detail-info">
                    <h2>${book.title}</h2>
                    <p class="book-detail-author">by ${book.author}</p>
                    <div class="ratings">
                        <i class="fa fa-star"></i><i class="fa fa-star"></i><i class="fa fa-star"></i><i class="fa fa-star"></i><i class="fa fa-star-half-alt"></i>
                        <span>(1,234 reviews)</span>
                    </div>
                    <p class="book-detail-description">${book.description}</p>
                    <div class="pricing-options">
                        <p><strong>PDF Copy:</strong> $${book.pricePDF.toFixed(2)}</p>
                        <p><strong>Physical Copy:</strong> $${book.pricePhysical.toFixed(2)}</p>
                        <p><strong>Borrow (14 days):</strong> $${(book.pricePDF * 0.25).toFixed(2)}</p>
                    </div>
                    <div class="action-buttons">
                        <button class="btn btn-primary" data-action="buy-pdf" data-id="${book.id}">Buy PDF</button>
                        <button class="btn btn-secondary" data-action="buy-physical" data-id="${book.id}">Buy Physical</button>
                        <button class="btn btn-success" data-action="borrow" data-id="${book.id}">Borrow</button>
                        <button class="btn" data-action="wishlist" data-id="${book.id}"><i class="fa fa-heart"></i> Add to Wishlist</button>
                    </div>
                </div>
            </div>
        `;
    };
    
    const renderCart = () => {
        cartItemsContainer.innerHTML = '';
        let total = 0;
        if (state.cart.length === 0) {
            cartItemsContainer.innerHTML = '<p>Your cart is empty.</p>';
        } else {
            state.cart.forEach(item => {
                const book = state.books.find(b => b.id === item.bookId);
                const price = item.type === 'pdf' ? book.pricePDF : book.pricePhysical;
                total += price;
                cartItemsContainer.innerHTML += `
                    <div class="cart-item">
                        <img src="${book.cover}" alt="${book.title}" class="cart-item-img">
                        <div class="cart-item-info">
                            <h4>${book.title}</h4>
                            <p>Format: ${item.type.toUpperCase()}</p>
                        </div>
                        <p class="cart-item-price">$${price.toFixed(2)}</p>
                    </div>
                `;
            });
        }
        cartTotalEl.textContent = `$${total.toFixed(2)}`;
        cartCount.textContent = state.cart.length;
    };

    const renderDashboard = () => {
        if (!state.currentUser) return;
        if (state.currentUser.type === 'customer') {
            dashboardPage.innerHTML = `
                <h2>Customer Dashboard</h2>
                <div class="dashboard-grid">
                    <div class="dashboard-card">
                        <h3>My Orders</h3>
                        <p>You have no recent orders.</p>
                    </div>
                    <div class="dashboard-card">
                        <h3>Currently Borrowed Books</h3>
                        <p><strong>The Midnight Library</strong> - Expires in 8 days <button class="btn btn-sm" id="read-now-btn">Read Now</button></p>
                        <p>You have no other borrowed books.</p>
                    </div>
                </div>`;
            document.getElementById('read-now-btn').addEventListener('click', () => ereaderModal.classList.remove('hidden'));

        } else if (state.currentUser.type === 'vendor') {
            dashboardPage.innerHTML = `
                <h2>Vendor Dashboard</h2>
                <div class="dashboard-grid">
                    <div class="dashboard-card">
                        <h3>Manage Books</h3>
                        <p>You have ${state.books.length} books listed.</p>
                        <button class="btn btn-primary">Add New Book</button>
                    </div>
                    <div class="dashboard-card">
                        <h3>Sales Report</h3>
                        <p><strong>Total Earnings:</strong> $1,234.56</p>
                        <p><strong>Most Popular Book:</strong> Project Hail Mary</p>
                        <a href="#">View Detailed Analytics</a>
                    </div>
                </div>`;
        }
    };

    // ===== UI & NAVIGATION =====
    const showPage = (pageId) => {
        pages.forEach(page => {
            page.classList.add('hidden');
        });
        document.getElementById(pageId)?.classList.remove('hidden');

        // Update active nav link
        navLinks.forEach(link => {
            link.classList.toggle('active', link.dataset.page === pageId);
        });
    };

    const updateNav = () => {
        if (state.currentUser) {
            document.querySelectorAll('.auth-required').forEach(el => el.style.display = 'inline-block');
            document.querySelectorAll('.guest-only').forEach(el => el.style.display = 'none');
            if (state.currentUser.type === 'customer') {
                document.querySelectorAll('.customer-only').forEach(el => el.style.display = 'inline-block');
            } else {
                 document.querySelectorAll('.customer-only').forEach(el => el.style.display = 'none');
            }
        } else {
            document.querySelectorAll('.auth-required').forEach(el => el.style.display = 'none');
            document.querySelectorAll('.guest-only').forEach(el => el.style.display = 'inline-block');
        }
    };


    // ===== EVENT LISTENERS =====
    loginForm.addEventListener('submit', (e) => {
        e.preventDefault();
        const email = document.getElementById('email').value;
        if (email.includes('vendor')) {
            state.currentUser = { type: 'vendor' };
            showPage('dashboard-page');
            renderDashboard();
        } else {
            state.currentUser = { type: 'customer' };
            showPage('home-page');
        }
        updateNav();
    });

    logoutBtn.addEventListener('click', (e) => {
        e.preventDefault();
        state.currentUser = null;
        state.cart = [];
        updateNav();
        renderCart();
        showPage('login-page');
    });

    navLinks.forEach(link => {
        link.addEventListener('click', (e) => {
            e.preventDefault();
            // ===== THIS IS THE FIX =====
            // Use e.currentTarget instead of e.target to ensure we get the <a> tag
            const pageId = e.currentTarget.dataset.page; 
            // ===========================
            
            if (pageId) {
                if (pageId === 'cart') renderCart();
                if (pageId === 'dashboard') renderDashboard();
                showPage(pageId);
            }
        });
    });

    bookDetailPage.addEventListener('click', (e) => {
        const action = e.target.dataset.action;
        const bookId = parseInt(e.target.dataset.id);
        if (!action || !bookId) return;

        if (action === 'buy-pdf' || action === 'buy-physical') {
            state.cart.push({ bookId: bookId, type: action.split('-')[1] });
            alert('Added to cart!');
            renderCart();
        } else if (action === 'borrow') {
            alert('Book borrowed for 14 days!');
        } else if (action === 'wishlist') {
            alert('Added to wishlist!');
        }
    });

    ereaderModal.addEventListener('click', (e) => {
        if (e.target.classList.contains('modal-overlay') || e.target.classList.contains('modal-close-btn')) {
            ereaderModal.classList.add('hidden');
        }
    });

    // ===== INITIALIZATION =====
    const init = () => {
        updateNav();
        renderBookGrid();
        showPage('login-page');
    };

    init();
});