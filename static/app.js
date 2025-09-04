/*
  Digital Library System - Front-end Prototype
  - Data model uses localStorage for demo persistence
  - Edit SAMPLE_DATA to adjust initial books, orders, branches
  - Key namespaces in localStorage:
    - dls_user: { id, name, email, type: 'reader'|'vendor' }
    - dls_books: Book[]
    - dls_cart: CartItem[]
    - dls_wishlist: string[] (book ids)
    - dls_borrows: BorrowItem[]
    - dls_orders: Order[]
    - dls_branches: Branch[] (vendor)
*/

// ---------- Sample Data ----------
const SAMPLE_DATA = (() => {
  const categories = [
    'Fiction','Non-fiction','Technology','Science','History','Business','Children','Comics'
  ];
  const authors = ['A. Clarke','I. Asimov','U. Le Guin','M. Atwood','T. Hardy','S. King','Y. Noah','A. Christie'];
  const covers = [1,2,3,4,5,6,7,8].map(n => `assets/cover-${n}.svg`);
  const books = Array.from({length: 16}).map((_,i)=>({
    id: `b${i+1}`,
    title: `Sample Book ${i+1}`,
    author: authors[i%authors.length],
    category: categories[i%categories.length],
    rating: (Math.round((Math.random()*2+3)*10)/10),
    pricePdf: +(Math.random()*15 + 3).toFixed(2),
    pricePhysical: +(Math.random()*25 + 10).toFixed(2),
    stock: Math.floor(Math.random()*15)+1,
    canBorrow: Math.random()>0.3,
    description: 'A compelling description for the sample book showcasing its unique value and storyline.',
    cover: covers[i%covers.length],
    vendorId: 'vendor-1'
  }));

  const orders = [];
  const branches = [{ id:'br-1', name:'Main Branch', address:'123 Library St', contact:'+1-202-555-0101' }];
  return { books, orders, branches };
})();

// ---------- Storage Helpers ----------
const store = {
  get(key, fallback){
    try{ const v = localStorage.getItem(key); return v? JSON.parse(v): fallback; }catch{ return fallback }
  },
  set(key, value){ localStorage.setItem(key, JSON.stringify(value)); }
};

function initStorage(){
  if(!store.get('dls_books')) store.set('dls_books', SAMPLE_DATA.books);
  if(!store.get('dls_orders')) store.set('dls_orders', SAMPLE_DATA.orders);
  if(!store.get('dls_branches')) store.set('dls_branches', SAMPLE_DATA.branches);
  if(!store.get('dls_cart')) store.set('dls_cart', []);
  if(!store.get('dls_wishlist')) store.set('dls_wishlist', []);
  if(!store.get('dls_borrows')) store.set('dls_borrows', []);
}

// ---------- State ----------
const state = {
  user: store.get('dls_user', null),
  view: 'catalog',
  catalogPage: 1,
  priceMax: 100,
  libraryTab: 'borrowed',
  reader: { pages: [], idx: 0, borrowInfo: null }
};

// ---------- UI Utilities ----------
function $(sel){ return document.querySelector(sel); }
function $all(sel){ return document.querySelectorAll(sel); }
function formatCurrency(n){ return `$${(+n).toFixed(2)}`; }

function showToast(message, type='primary'){
  const id = `t${Date.now()}`;
  const el = document.createElement('div');
  el.className = `toast align-items-center text-bg-${type} border-0`;
  el.id = id;
  el.role = 'status';
  el.ariaLive = 'polite';
  el.innerHTML = `<div class="d-flex"><div class="toast-body">${message}</div><button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button></div>`;
  $('#toastContainer').appendChild(el);
  const t = bootstrap.Toast.getOrCreateInstance(el, { delay: 2500 });
  t.show();
}

function setView(name){
  state.view = name;
  $all('.view').forEach(v => v.classList.add('d-none'));
  $(`#view-${name}`).classList.remove('d-none');
  $all('[data-nav]').forEach(a=> a.classList.toggle('active', a.getAttribute('data-nav')===name));
}

function updateAuthUI(){
  const isLogged = !!state.user;
  $('#authDisplay').textContent = isLogged ? `${state.user.name} (${state.user.type})` : 'Login / Register';
  $('#logoutBtn').classList.toggle('d-none', !isLogged);
  $all('.vendor-only').forEach(el=> el.classList.toggle('d-none', !(isLogged && state.user.type==='vendor')));
}

function refreshCartUI(){
  const cart = store.get('dls_cart', []);
  $('#cartCount').textContent = cart.length;
  const items = $('#cartItems');
  items.innerHTML = '';
  let total = 0;
  cart.forEach(ci=>{
    const book = getBooks().find(b=>b.id===ci.bookId);
    const price = ci.format==='pdf'? book.pricePdf : book.pricePhysical;
    total += price * (ci.qty||1);
    const div = document.createElement('div');
    div.className = 'd-flex align-items-center gap-2 py-2 border-bottom';
    div.innerHTML = `
      <img src="${book.cover}" width="40" height="56" class="rounded" alt="${book.title} cover">
      <div class="flex-grow-1">
        <div class="small fw-semibold">${book.title}</div>
        <div class="small text-muted">${ci.format.toUpperCase()} × ${ci.qty||1}</div>
      </div>
      <div class="small">${formatCurrency(price)}</div>
      <button class="btn btn-sm btn-outline-danger" aria-label="Remove" data-remove="${book.id}|${ci.format}">×</button>
    `;
    items.appendChild(div);
  });
  $('#cartEmpty').classList.toggle('d-none', cart.length>0);
  $('#checkoutBtn').disabled = cart.length===0;
  $('#cartTotal').textContent = formatCurrency(total);

  // remove handlers
  items.querySelectorAll('[data-remove]').forEach(btn=>{
    btn.addEventListener('click',()=>{
      const [bookId, format] = btn.getAttribute('data-remove').split('|');
      const cartNow = store.get('dls_cart', []);
      store.set('dls_cart', cartNow.filter(x=>!(x.bookId===bookId && x.format===format)));
      refreshCartUI();
      showToast('Removed from cart','warning');
    });
  })
}

function getBooks(){ return store.get('dls_books', []); }

// ---------- Catalog Rendering ----------
function populateFilters(){
  const books = getBooks();
  const categories = [...new Set(books.map(b=>b.category))];
  const authors = [...new Set(books.map(b=>b.author))];
  $('#filterCategory').innerHTML = '<option value="">All Categories</option>' + categories.map(c=>`<option>${c}</option>`).join('');
  $('#filterAuthor').innerHTML = '<option value="">All Authors</option>' + authors.map(a=>`<option>${a}</option>`).join('');
}

function passFilters(b){
  const cat = $('#filterCategory').value;
  const au = $('#filterAuthor').value;
  const fmt = $('#filterFormat').value;
  const priceMax = state.priceMax;
  const price = Math.min(b.pricePdf, b.pricePhysical);
  if(cat && b.category!==cat) return false;
  if(au && b.author!==au) return false;
  if(fmt==='pdf' && !b.pricePdf) return false;
  if(fmt==='physical' && !b.pricePhysical) return false;
  if(price > priceMax) return false;
  return true;
}

function sortBooks(arr){
  const s = $('#sortSelect').value;
  if(s==='price_asc') return arr.sort((a,b)=>Math.min(a.pricePdf,a.pricePhysical)-Math.min(b.pricePdf,b.pricePhysical));
  if(s==='price_desc') return arr.sort((a,b)=>Math.min(b.pricePdf,b.pricePhysical)-Math.min(a.pricePdf,a.pricePhysical));
  if(s==='newest') return arr.slice().reverse();
  return arr; // popularity mock
}

function renderCatalog(reset=false){
  const grid = $('#catalogGrid');
  if(reset){ grid.innerHTML=''; state.catalogPage=1; }
  const pageSize = 8;
  const filtered = sortBooks(getBooks().filter(passFilters));
  const start = (state.catalogPage-1)*pageSize;
  const pageItems = filtered.slice(start, start+pageSize);
  pageItems.forEach(b=>{
    const col = document.createElement('div');
    col.innerHTML = `
      <div class="card h-100">
        <img src="${b.cover}" alt="${b.title} cover" class="card-img-top card-cover">
        <div class="card-body d-flex flex-column">
          <h6 class="card-title mb-1 text-truncate" title="${b.title}">${b.title}</h6>
          <div class="small text-muted text-truncate" title="${b.author}">${b.author}</div>
          <div class="mt-2 small"><span class="rating">★</span> ${b.rating}</div>
          <div class="mt-2 small d-flex gap-2"><span>${formatCurrency(b.pricePdf)} PDF</span><span>${formatCurrency(b.pricePhysical)} Physical</span></div>
          <div class="mt-auto d-grid gap-2">
            <div class="btn-group" role="group">
              <button class="btn btn-outline-secondary btn-sm" data-preview="${b.id}">Preview</button>
              <button class="btn btn-outline-primary btn-sm" data-wish="${b.id}">Wishlist</button>
            </div>
            <div class="btn-group" role="group">
              <button class="btn btn-primary btn-sm" data-addcart="${b.id}|pdf">Buy PDF</button>
              <button class="btn btn-primary btn-sm" data-addcart="${b.id}|physical">Buy Physical</button>
              <button class="btn btn-success btn-sm" data-borrow="${b.id}" ${b.canBorrow? '':'disabled'}>Borrow</button>
            </div>
            <button class="btn btn-light btn-sm" data-detail="${b.id}">Details</button>
          </div>
        </div>
      </div>
    `;
    grid.appendChild(col);
  });
  $('#loadMoreBtn').classList.toggle('d-none', start+pageSize >= filtered.length);

  // bind buttons
  grid.querySelectorAll('[data-preview]').forEach(btn=> btn.onclick = ()=> openPreview(btn.getAttribute('data-preview')));
  grid.querySelectorAll('[data-wish]').forEach(btn=> btn.onclick = ()=> addToWishlist(btn.getAttribute('data-wish')));
  grid.querySelectorAll('[data-addcart]').forEach(btn=> btn.onclick = ()=>{
    const [bookId, fmt] = btn.getAttribute('data-addcart').split('|');
    addToCart(bookId, fmt);
  });
  grid.querySelectorAll('[data-borrow]').forEach(btn=> btn.onclick = ()=> openBorrow(btn.getAttribute('data-borrow')));
  grid.querySelectorAll('[data-detail]').forEach(btn=> btn.onclick = ()=> openDetail(btn.getAttribute('data-detail')));
}

// ---------- Actions ----------
function addToCart(bookId, format){
  const cart = store.get('dls_cart', []);
  const existing = cart.find(c=>c.bookId===bookId && c.format===format);
  if(existing){ existing.qty = (existing.qty||1)+1; }
  else cart.push({ bookId, format, qty:1 });
  store.set('dls_cart', cart);
  refreshCartUI();
  showToast('Added to cart','success');
}

function addToWishlist(bookId){
  const wl = new Set(store.get('dls_wishlist', []));
  wl.add(bookId);
  store.set('dls_wishlist', Array.from(wl));
  showToast('Added to wishlist','primary');
}

function openPreview(bookId){
  const book = getBooks().find(b=>b.id===bookId);
  $('#previewContent').innerHTML = [1,2,3].map(n=>`<div class="preview-page">${book.title} — Preview page ${n}. Lorem ipsum dolor sit amet, consectetur adipiscing elit.</div>`).join('');
  const modal = new bootstrap.Modal($('#previewModal'));
  modal.show();
}

// Detail
function openDetail(bookId){
  const book = getBooks().find(b=>b.id===bookId);
  $('#bookDetailTitle').textContent = book.title;
  $('#bookDetailAuthor').textContent = book.author;
  $('#bookDetailRating').textContent = `★ ${book.rating}`;
  $('#bookDetailDesc').textContent = book.description;
  $('#bookDetailCover').src = book.cover;
  $('#detailPreviewBtn').onclick = ()=> openPreview(bookId);
  $('#detailWishlistBtn').onclick = ()=> addToWishlist(bookId);
  $('#detailBorrowBtn').disabled = !book.canBorrow;
  $('#detailBorrowBtn').onclick = ()=> openBorrow(bookId);
  $('#detailBuyPdfBtn').onclick = ()=> addToCart(bookId,'pdf');
  $('#detailBuyPhysicalBtn').onclick = ()=> addToCart(bookId,'physical');
  $('#reviewList').innerHTML = getReviews(bookId).map(r=>`<li class="list-group-item">${'★'.repeat(r.rating)} - ${r.text} <span class="text-muted">— ${r.user}</span></li>`).join('') || '<li class="list-group-item text-muted">No reviews yet.</li>';
  $('#reviewForm').onsubmit = (e)=>{
    e.preventDefault();
    const rating = +$('#reviewRating').value;
    const text = $('#reviewText').value.trim();
    if(!state.user){ showToast('Login required to post reviews','danger'); return; }
    if(text.length<2){ showToast('Please write a short review','warning'); return; }
    postReview(bookId, { rating, text, user: state.user.name });
    $('#reviewText').value = '';
    openDetail(bookId);
    showToast('Review posted','success');
  };
  new bootstrap.Modal($('#bookDetailModal')).show();
}

function getReviewsKey(bookId){ return `dls_reviews_${bookId}`; }
function getReviews(bookId){ return store.get(getReviewsKey(bookId), []); }
function postReview(bookId, review){
  const arr = getReviews(bookId);
  arr.unshift({ ...review, ts: Date.now() });
  store.set(getReviewsKey(bookId), arr);
}

// Borrow
let currentBorrowBookId = null;
function openBorrow(bookId){
  currentBorrowBookId = bookId;
  new bootstrap.Modal($('#borrowModal')).show();
}

$('#borrowForm')?.addEventListener('submit', (e)=>{
  e.preventDefault();
  if(!state.user){ showToast('Please login to borrow','danger'); return; }
  const days = +document.querySelector('input[name="borrowDuration"]:checked').value;
  const expiresAt = Date.now() + days*24*60*60*1000 / 96; // demo speed-up (~15 min per 7 days -> compressed)
  const borrows = store.get('dls_borrows', []);
  borrows.push({ bookId: currentBorrowBookId, userId: state.user.id||state.user.email, expiresAt });
  store.set('dls_borrows', borrows);
  bootstrap.Modal.getInstance($('#borrowModal')).hide();
  showToast(`Borrowed for ${days} days (demo timer)`,`success`);
});

// Reader
function openReader(bookId, borrowInfo=null){
  const book = getBooks().find(b=>b.id===bookId);
  state.reader.pages = Array.from({length: 8}).map((_,i)=>`<h4>${book.title}</h4><p>Page ${i+1} — Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed ut perspiciatis unde omnis iste natus error sit voluptatem.</p>`);
  state.reader.idx = 0;
  state.reader.borrowInfo = borrowInfo;
  $('#readerTitle').textContent = book.title;
  $('#readerTotal').textContent = state.reader.pages.length;
  $('#readerWatermark').classList.toggle('d-none', !borrowInfo);
  $('#readerWatermark').textContent = borrowInfo? `${state.user?.email||'guest'} — Expires ${new Date(borrowInfo.expiresAt).toLocaleString()}` : '';
  $('#readerBorrowInfo').classList.toggle('d-none', !borrowInfo);
  if(borrowInfo) $('#readerBorrowInfo').textContent = `Expires in ${timeLeft(borrowInfo.expiresAt)}`;
  updateReaderPage();
  const isBorrowedActive = borrowInfo && Date.now() < borrowInfo.expiresAt;
  $('#readerDownload').disabled = !!borrowInfo; // disable for borrowed
  $('#readerArea').classList.toggle('drm-disabled', !!borrowInfo);
  $('#readerArea').oncontextmenu = (e)=>{ if(borrowInfo) e.preventDefault(); };
  new bootstrap.Modal($('#readerModal')).show();
}

function updateReaderPage(){
  $('#readerPage').textContent = state.reader.idx+1;
  $('#readerPageContent').innerHTML = state.reader.pages[state.reader.idx];
}
$('#readerPrev').addEventListener('click', ()=>{ if(state.reader.idx>0){ state.reader.idx--; updateReaderPage(); }});
$('#readerNext').addEventListener('click', ()=>{ if(state.reader.idx<state.reader.pages.length-1){ state.reader.idx++; updateReaderPage(); }});

// Library rendering
function renderLibrary(){
  const list = $('#libraryList');
  list.innerHTML = '';
  const borrows = store.get('dls_borrows', []).filter(b=> state.user && (b.userId=== (state.user.id||state.user.email)));
  const purchases = store.get('dls_orders', []).filter(o=> o.userEmail === state.user?.email);
  const items = state.libraryTab==='borrowed' ? borrows : purchases;
  $('#libraryEmpty').classList.toggle('d-none', items.length>0);
  items.forEach(item=>{
    const isBorrow = !!item.expiresAt;
    const book = getBooks().find(b=>b.id=== (isBorrow? item.bookId : item.bookId));
    const expired = isBorrow && Date.now() > item.expiresAt;
    const card = document.createElement('div');
    card.innerHTML = `
      <div class="card h-100">
        <div class="row g-0">
          <div class="col-4"><img src="${book.cover}" class="img-fluid rounded-start" alt="${book.title} cover"></div>
          <div class="col-8">
            <div class="card-body">
              <h6 class="card-title">${book.title}</h6>
              ${isBorrow ? `<div class="small ${expired?'text-danger':'text-muted'}">${expired? 'Expired' : 'Expires in '+timeLeft(item.expiresAt)}</div>` : `<div class="small text-muted">Purchased (${item.format?.toUpperCase()||'PDF'})</div>`}
              <div class="d-flex gap-2 mt-2">
                <button class="btn btn-sm btn-primary" ${expired? 'disabled':''} data-read="${book.id}">Read</button>
                ${isBorrow? `<button class="btn btn-sm btn-outline-success" data-extend="${book.id}">Extend</button>`: ''}
                ${!isBorrow? `<a class="btn btn-sm btn-outline-secondary" download href="#" data-download="${book.id}">Download</a>`:''}
              </div>
            </div>
          </div>
        </div>
      </div>`;
    list.appendChild(card);
  });

  list.querySelectorAll('[data-read]').forEach(btn=>{
    btn.addEventListener('click', ()=>{
      const bookId = btn.getAttribute('data-read');
      const borrow = store.get('dls_borrows', []).find(b=>b.bookId===bookId && (b.userId=== (state.user.id||state.user.email)));
      openReader(bookId, borrow||null);
    });
  });
  list.querySelectorAll('[data-extend]').forEach(btn=>{
    btn.addEventListener('click', ()=>{
      const bookId = btn.getAttribute('data-extend');
      const borrows = store.get('dls_borrows', []);
      const idx = borrows.findIndex(b=>b.bookId===bookId && (b.userId===(state.user.id||state.user.email)));
      if(idx>-1){ borrows[idx].expiresAt += 3*60*1000; store.set('dls_borrows', borrows); showToast('Borrow extended','success'); renderLibrary(); }
    });
  });
}

function timeLeft(ts){
  const ms = Math.max(0, ts - Date.now());
  const m = Math.floor(ms/60000); const s = Math.floor((ms%60000)/1000);
  return `${m}m ${s}s`;
}

// Auth
$('#registerForm')?.addEventListener('submit', (e)=>{
  e.preventDefault();
  const name = $('#regName').value.trim();
  const email = $('#regEmail').value.trim();
  const password = $('#regPassword').value;
  const type = $('#regType').value;
  if(!name || !email || password.length<6){ $('#registerFeedback').classList.remove('d-none'); return; }
  const user = { id: `u-${Date.now()}`, name, email, type };
  store.set('dls_user', user); state.user = user;
  bootstrap.Modal.getInstance($('#registerModal')).hide();
  updateAuthUI(); showToast('Registered & logged in','success');
});

$('#loginForm')?.addEventListener('submit', (e)=>{
  e.preventDefault();
  const email = $('#loginEmail').value.trim();
  const password = $('#loginPassword').value;
  if(!email || !password){ $('#loginFeedback').classList.remove('d-none'); return; }
  const remember = $('#rememberMe').checked;
  const user = { id:`u-${Date.now()}`, name: email.split('@')[0], email, type: 'reader' };
  store.set('dls_user', user); state.user = user;
  if(remember) localStorage.setItem('dls_last_email', email);
  bootstrap.Modal.getInstance($('#loginModal')).hide();
  updateAuthUI(); showToast('Logged in','success');
});

$('#logoutBtn')?.addEventListener('click',(e)=>{
  e.preventDefault();
  localStorage.removeItem('dls_user'); state.user = null; updateAuthUI(); showToast('Logged out','warning');
});

// Nav
$all('[data-nav]').forEach(a=>{
  a.addEventListener('click', (e)=>{
    e.preventDefault();
    const name = a.getAttribute('data-nav');
    if(name==='vendor' && !(state.user&&state.user.type==='vendor')){ showToast('Vendor access only','danger'); return; }
    setView(name);
    if(name==='catalog') renderCatalog(true);
    if(name==='mylibrary') renderLibrary();
  })
});

// Global search
$('#globalSearchBtn').addEventListener('click', ()=>{
  const q = $('#globalSearchInput').value.trim().toLowerCase();
  if(q){
    setView('catalog');
    $('#catalogGrid').innerHTML = '';
    const results = getBooks().filter(b=> (b.title+b.author+b.category).toLowerCase().includes(q));
    sortBooks(results).forEach(b=>{
      const div = document.createElement('div');
      div.innerHTML = `<div class="card h-100"><img src="${b.cover}" class="card-img-top card-cover" alt="${b.title} cover"><div class="card-body"><h6 class="card-title">${b.title}</h6><div class="small text-muted">${b.author}</div></div></div>`;
      $('#catalogGrid').appendChild(div);
    });
  }
});

// Filters
$('#filterCategory').addEventListener('change', ()=> renderCatalog(true));
$('#filterAuthor').addEventListener('change', ()=> renderCatalog(true));
$('#filterFormat').addEventListener('change', ()=> renderCatalog(true));
$('#sortSelect').addEventListener('change', ()=> renderCatalog(true));
$('#priceRange').addEventListener('input', ()=>{
  state.priceMax = +$('#priceRange').value;
  $('#priceRangeValue').textContent = `$${state.priceMax}`;
});
$('#priceRange').addEventListener('change', ()=> renderCatalog(true));
$('#loadMoreBtn').addEventListener('click', ()=>{ state.catalogPage++; renderCatalog(false); });

// Checkout (mock payment)
$('#paymentMethods').addEventListener('click', (e)=>{
  const btn = e.target.closest('button[data-method]');
  if(!btn) return;
  const method = btn.getAttribute('data-method');
  const cart = store.get('dls_cart', []);
  if(cart.length===0){ showToast('Cart is empty','warning'); return; }
  // create orders for physical downloads and PDFs
  const orders = store.get('dls_orders', []);
  cart.forEach(ci=>{
    orders.push({ id:`o-${Date.now()}-${Math.random().toString(36).slice(2,6)}`, userEmail: state.user?.email||'guest@example.com', bookId: ci.bookId, qty: ci.qty||1, format: ci.format, status: 'Pending' });
  });
  store.set('dls_orders', orders);
  store.set('dls_cart', []);
  refreshCartUI();
  bootstrap.Modal.getInstance($('#checkoutModal'))?.hide();
  showToast(`Payment simulated via ${method}. Order placed!`,'success');
});

// Vendor CRUD (basic)
function renderVendorTables(){
  const books = getBooks();
  const tbody = $('#vendorBooksTable');
  tbody.innerHTML = '';
  books.forEach(b=>{
    const tr = document.createElement('tr');
    tr.innerHTML = `
      <td><img src="${b.cover}" class="table-cover" alt="${b.title} cover"></td>
      <td>${b.title}</td>
      <td>${b.author}</td>
      <td>${formatCurrency(b.pricePdf)}</td>
      <td>${formatCurrency(b.pricePhysical)}</td>
      <td>${b.stock}</td>
      <td>${b.canBorrow? 'Yes':'No'}</td>
      <td class="text-end">
        <button class="btn btn-sm btn-outline-secondary" data-edit="${b.id}">Edit</button>
        <button class="btn btn-sm btn-outline-danger" data-del="${b.id}">Delete</button>
      </td>`;
    tbody.appendChild(tr);
  });

  tbody.querySelectorAll('[data-del]').forEach(btn=> btn.onclick = ()=>{
    const id = btn.getAttribute('data-del');
    const booksNow = getBooks().filter(x=>x.id!==id);
    store.set('dls_books', booksNow);
    renderVendorTables(); renderCatalog(true);
    showToast('Book deleted','warning');
  });

  tbody.querySelectorAll('[data-edit]').forEach(btn=> btn.onclick = ()=> openBookForm(btn.getAttribute('data-edit')));

  // Orders
  const orders = store.get('dls_orders', []);
  const otbody = $('#vendorOrdersTable');
  otbody.innerHTML = '';
  orders.forEach(o=>{
    const book = getBooks().find(b=>b.id===o.bookId);
    const tr = document.createElement('tr');
    tr.innerHTML = `
      <td>${o.id}</td>
      <td>${book?.title||o.bookId}</td>
      <td>${o.userEmail}</td>
      <td>${o.qty}</td>
      <td><span class="badge text-bg-secondary">${o.status}</span></td>
      <td class="text-end"><button class="btn btn-sm btn-outline-primary" data-advance="${o.id}">Advance Status</button></td>`;
    otbody.appendChild(tr);
  });
  otbody.querySelectorAll('[data-advance]').forEach(btn=> btn.onclick = ()=>{
    const id = btn.getAttribute('data-advance');
    const ordersNow = store.get('dls_orders', []);
    const idx = ordersNow.findIndex(x=>x.id===id);
    const flow = ['Pending','Shipped','Delivered'];
    const cur = ordersNow[idx].status;
    const next = flow[(flow.indexOf(cur)+1)%flow.length];
    ordersNow[idx].status = next;
    store.set('dls_orders', ordersNow);
    renderVendorTables();
    showToast(`Order ${id} -> ${next}`,'success');
  });

  // Reports small demo
  if(window.Chart){
    if(renderVendorTables._chart) renderVendorTables._chart.destroy();
    const ctx = $('#chartSales');
    renderVendorTables._chart = new Chart(ctx, {
      type:'bar', data:{ labels:['Mon','Tue','Wed','Thu','Fri','Sat','Sun'], datasets:[{ label:'Sales', data:[3,2,5,4,6,7,3], backgroundColor:'rgba(13,110,253,.6)' }] }, options:{ responsive:true, plugins:{ legend:{ display:false } } }
    });
  }

  // Branches
  const branches = store.get('dls_branches', []);
  const blist = $('#branchList');
  blist.innerHTML = '';
  branches.forEach(b=>{
    const li = document.createElement('li');
    li.className = 'list-group-item d-flex justify-content-between align-items-start';
    li.innerHTML = `<div><div>${b.name}</div><div class="text-muted small">${b.address} — ${b.contact}</div></div><button class="btn btn-sm btn-outline-danger" data-del-branch="${b.id}">Delete</button>`;
    blist.appendChild(li);
  });
  blist.querySelectorAll('[data-del-branch]').forEach(btn=> btn.onclick = ()=>{
    const id = btn.getAttribute('data-del-branch');
    store.set('dls_branches', store.get('dls_branches', []).filter(x=>x.id!==id));
    renderVendorTables(); showToast('Branch removed','warning');
  });
}

function openBookForm(bookId){
  const b = getBooks().find(x=>x.id===bookId);
  const wrapper = document.createElement('div');
  wrapper.innerHTML = `
    <div class="modal fade" id="bookFormModal" tabindex="-1"><div class="modal-dialog"><div class="modal-content">
      <div class="modal-header"><h5 class="modal-title">${b? 'Edit':'Add'} Book</h5><button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button></div>
      <form id="bookForm">
        <div class="modal-body">
          <div class="row g-2">
            <div class="col-12"><label class="form-label">Title<input class="form-control" id="bfTitle" required value="${b?.title||''}"></label></div>
            <div class="col-12 col-md-6"><label class="form-label">Author<input class="form-control" id="bfAuthor" required value="${b?.author||''}"></label></div>
            <div class="col-12 col-md-6"><label class="form-label">Category<input class="form-control" id="bfCategory" required value="${b?.category||''}"></label></div>
            <div class="col-6"><label class="form-label">Price PDF<input type="number" min="0" step="0.01" class="form-control" id="bfPdf" value="${b?.pricePdf||0}"></label></div>
            <div class="col-6"><label class="form-label">Price Physical<input type="number" min="0" step="0.01" class="form-control" id="bfPhysical" value="${b?.pricePhysical||0}"></label></div>
            <div class="col-6"><label class="form-label">Stock<input type="number" min="0" class="form-control" id="bfStock" value="${b?.stock||0}"></label></div>
            <div class="col-6"><label class="form-label">Borrow Available<select class="form-select" id="bfBorrow"><option value="true" ${b?.canBorrow?'selected':''}>Yes</option><option value="false" ${b && !b.canBorrow?'selected':''}>No</option></select></label></div>
            <div class="col-12"><label class="form-label">Cover URL<input class="form-control" id="bfCover" value="${b?.cover||'assets/cover-1.svg'}"></label></div>
            <div class="col-12"><label class="form-label">Description<textarea class="form-control" id="bfDesc" rows="3">${b?.description||''}</textarea></label></div>
          </div>
        </div>
        <div class="modal-footer"><button type="button" class="btn btn-outline-secondary" data-bs-dismiss="modal">Cancel</button><button type="submit" class="btn btn-primary">Save</button></div>
      </form>
    </div></div></div>`;
  document.body.appendChild(wrapper);
  const modalEl = wrapper.querySelector('#bookFormModal');
  const modal = new bootstrap.Modal(modalEl);
  modal.show();
  wrapper.querySelector('#bookForm').addEventListener('submit', (e)=>{
    e.preventDefault();
    const updated = {
      id: b?.id || `b-${Date.now()}`,
      title: $('#bfTitle').value.trim(),
      author: $('#bfAuthor').value.trim(),
      category: $('#bfCategory').value.trim(),
      pricePdf: +$('#bfPdf').value||0,
      pricePhysical: +$('#bfPhysical').value||0,
      stock: +$('#bfStock').value||0,
      canBorrow: $('#bfBorrow').value==='true',
      cover: $('#bfCover').value.trim(),
      description: $('#bfDesc').value.trim(),
      rating: b?.rating || 4.2,
      vendorId: state.user?.id || 'vendor-1'
    };
    const arr = getBooks();
    const exists = arr.findIndex(x=>x.id===updated.id);
    if(exists>-1) arr[exists]=updated; else arr.push(updated);
    store.set('dls_books', arr);
    modal.hide(); wrapper.remove();
    renderVendorTables(); renderCatalog(true);
    showToast('Book saved','success');
  });
  modalEl.addEventListener('hidden.bs.modal', ()=> wrapper.remove());
}

$('#vendorAddBookBtn')?.addEventListener('click', ()=> openBookForm(null));
$('#vendorAddBranchBtn')?.addEventListener('click', ()=>{
  const name = prompt('Branch name'); if(!name) return;
  const address = prompt('Address')||''; const contact = prompt('Contact')||'';
  const branches = store.get('dls_branches', []);
  branches.push({ id:`br-${Date.now()}`, name, address, contact });
  store.set('dls_branches', branches); renderVendorTables(); showToast('Branch added','success');
});

// Subscription and institutional
$('#subscribeBtn')?.addEventListener('click', ()=> showToast('Subscribed! (demo)','success'));
$('#institutionForm')?.addEventListener('submit', (e)=>{ e.preventDefault(); showToast('Request submitted. We will contact you.','success'); e.target.reset(); });

// Misc UI
$('#yearNow').textContent = new Date().getFullYear();
$('#darkModeSwitch').addEventListener('change', (e)=>{ document.body.classList.toggle('dark', e.target.checked); });

// Initialize
document.addEventListener('DOMContentLoaded', ()=>{
  initStorage();
  state.user = store.get('dls_user', null);
  updateAuthUI();
  populateFilters();
  $('#priceRange').value = state.priceMax; $('#priceRangeValue').textContent = `$${state.priceMax}`;
  renderCatalog(true);
  refreshCartUI();
  renderVendorTables();

  // Vendor stats demo
  const orders = store.get('dls_orders', []);
  $('#vendorSales').textContent = orders.length;
  $('#vendorBorrowed').textContent = store.get('dls_borrows', []).length;
  const revenue = orders.reduce((sum,o)=>{
    const b = getBooks().find(x=>x.id===o.bookId);
    return sum + (o.format==='pdf'? b?.pricePdf||0 : b?.pricePhysical||0);
  },0);
  $('#vendorRevenue').textContent = formatCurrency(revenue);

  // Library countdown refresh
  setInterval(()=>{ if(state.view==='mylibrary') renderLibrary(); }, 1000);
});


