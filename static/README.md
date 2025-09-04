## Digital Library System — UI Prototype (Bootstrap 5)

This is a front-end only, responsive prototype for a Digital Library System built with plain HTML, CSS, JavaScript, and Bootstrap 5. It simulates catalog browsing, authentication, cart/checkout, borrowing with timers, an e-reader with basic DRM UX, and a vendor dashboard with CRUD, orders, and simple reports.

### How to run

- Open `index.html` in a modern browser.
- Optional local server for best results with assets:
  - Python: `python -m http.server` then open `http://localhost:8000`
  - Any static server (e.g., VS Code Live Server)

### Structure

- `index.html` — Single-page app shell (Catalog, My Library, Vendor, Subscription, Institutional)
- `styles.css` — Custom styles (watermark, dark mode, helpers)
- `app.js` — Interactions, sample data, localStorage persistence
- `assets/` — Icons and placeholder covers

### Features

- Catalog with filters, price slider, sorting, and pagination mock
- Global search in navbar
- Auth modals (Login, Register) with session state; vendor link when vendor
- Wishlist, Cart (offcanvas), Checkout with mock payment methods
- Book Detail with Reviews (post rating + text)
- Preview modal with watermark
- Borrow flow with compressed timers for demo; extend option
- My Library: Borrowed vs Purchased; open in e-reader
- E-Reader: pagination, watermark for borrowed, disabled download and right-click
- Vendor Dashboard: summary, Manage Books CRUD, Orders status flow, Branches
- Reports: Chart.js bar chart and popular list placeholder
- Subscription CTA and Institutional quote form
- Toast notifications; empty states; accessibility basics

### Demo data and persistence

- Seeded on first load and stored in `localStorage` under: `dls_books`, `dls_orders`, `dls_branches`, `dls_cart`, `dls_wishlist`, `dls_borrows`, `dls_user`.
- Reset by clearing site storage.

#### Data models (see `app.js`)

- Book: `{ id, title, author, category, rating, pricePdf, pricePhysical, stock, canBorrow, description, cover, vendorId }`
- CartItem: `{ bookId, format, qty }`
- BorrowItem: `{ bookId, userId, expiresAt }`
- Order: `{ id, userEmail, bookId, qty, format, status }`
- Branch: `{ id, name, address, contact }`

### Usage guide

1. Register or Login (choose Reader or Vendor). Vendor gets a dashboard link.
2. Browse Catalog, use filters/sort/price. Preview, Wishlist, Add to Cart, or Borrow.
3. Open Cart and Checkout. Pick any payment to simulate success; orders persist.
4. Borrow: choose 7 or 14 days (compressed timer). Extend from My Library.
5. My Library: Borrowed or Purchased; click Read to open e-reader. Borrowed disables download and shows watermark.
6. Vendor Dashboard: Add/Edit/Delete books, advance order statuses, add branches, view chart.

### Testing checklist

- Auth toggles UI and vendor link
- Search/filters return expected results
- Cart and checkout simulate payment and clear cart
- Borrow sets countdown; expiry disables reading; extend works
- Vendor CRUD persists; Orders advance status
- Wishlist and reviews persist
- E-reader watermark and right-click block for borrowed

Notes: Chart.js via CDN. Dark mode switch in footer.


