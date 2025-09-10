(function ($) {
    "use strict";
    
    $(document).ready(function () {
        console.log("Document is ready")
        // Spinner
        var spinner = function () {
            setTimeout(function () {
                if ($('#spinner').length > 0) {
                    $('#spinner').removeClass('show');
                }
            }, 1);
        };
        spinner();

        // Back to top button
        $(window).scroll(function () {
            if ($(this).scrollTop() > 300) {
                $('.back-to-top').fadeIn('slow');
            } else {
                $('.back-to-top').fadeOut('slow');
            }
        });

        $('.back-to-top').click(function () {
            $('html, body').animate({ scrollTop: 0 }, 1500); // <-- تم حذف 'easeInOutExpo'
            return false;
        });


        // Sidebar Toggler
        $('.sidebar-toggler').on('click', function () {
            $('.sidebar, .content').toggleClass("open");
            // نرجع false لمنع السلوك الافتراضي للرابط (وهو الانتقال لأعلى الصفحة)
            return false;
        });

        // Progress Bar
        $('.pg-bar').waypoint(function () {
            $('.progress .progress-bar').each(function () {
                $(this).css("width", $(this).attr("aria-valuenow") + '%');
            });
        }, {offset: '80%'});


        // Calender
        $('#calender').datetimepicker({
            inline: true,
            format: 'L'
        });


        // Testimonials carousel
        $(".testimonial-carousel").owlCarousel({
            autoplay: true,
            smartSpeed: 1000,
            items: 1,
            dots: true,
            loop: true,
            nav : false
        });
    }); // أغلق دالة ready

})(jQuery);

