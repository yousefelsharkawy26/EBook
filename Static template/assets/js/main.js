document.addEventListener('DOMContentLoaded', function(){
  // Sticky navbar shadow on scroll
  var nav = document.getElementById('siteNav');
  var backToTop = document.getElementById('backToTop');
  var yearSpan = document.getElementById('year');
  if(yearSpan){ yearSpan.textContent = new Date().getFullYear(); }

  function onScroll(){
    if(window.scrollY > 10){
      nav && nav.classList.add('shadow-sm');
      backToTop && backToTop.classList.add('show');
    } else {
      nav && nav.classList.remove('shadow-sm');
      backToTop && backToTop.classList.remove('show');
    }
  }
  window.addEventListener('scroll', onScroll);
  onScroll();

  // Smooth scroll for internal links
  document.querySelectorAll('a[href^="#"]').forEach(function(anchor){
    anchor.addEventListener('click', function(e){
      var targetId = this.getAttribute('href');
      if(targetId && targetId.length > 1){
        var el = document.querySelector(targetId);
        if(el){
          e.preventDefault();
          window.scrollTo({
            top: el.getBoundingClientRect().top + window.scrollY - 72,
            behavior: 'smooth'
          });
        }
      }
    });
  });

  // Simple search filter for cards
  var input = document.getElementById('searchInput');
  var grid = document.getElementById('bookGrid');
  if(input && grid){
    input.addEventListener('input', function(){
      var q = this.value.trim().toLowerCase();
      grid.querySelectorAll('.card').forEach(function(card){
        var text = card.textContent.toLowerCase();
        var col = card.closest('[class^="col-"]') || card.parentElement;
        if(text.indexOf(q) !== -1){
          col.style.display = '';
        } else {
          col.style.display = 'none';
        }
      });
    });
  }
});


