window.onload = function () {
    const hamburger = document.getElementById('Bar');
    const navMenu = document.getElementById('Menu');
    const overlay = document.getElementById('overlay');
    const closeBtn = document.getElementById('Close');

    // باز و بسته کردن منو
    hamburger.addEventListener('click', (event) => {
        event.stopPropagation(); // جلوگیری از بسته شدن هنگام کلیک روی همبرگر

        if (navMenu.classList.contains('active')) {
            navMenu.classList.remove('active');
            overlay.classList.remove('active'); // مخفی کردن لایه تیره
            setTimeout(() => {
                navMenu.style.display = 'none'; // بعد از انیمیشن مخفی شود
            }, 500); // مدت زمان انیمیشن
        } else {
            navMenu.style.display = 'block';
            overlay.style.display = 'block'; // نمایش منو قبل از شروع انیمیشن
            setTimeout(() => {
                navMenu.classList.add('active');
            }, 10); // کمی تأخیر برای شروع انیمیشن
        }
    });

    // بستن منو هنگام کلیک بیرون از آن
    document.addEventListener('click', (event) => {
        if (!navMenu.contains(event.target) && !hamburger.contains(event.target)) {
            if (navMenu.classList.contains('active')) {
                navMenu.classList.remove('active');
                overlay.style.display = 'none';
                setTimeout(() => {
                    navMenu.style.display = 'none';
                }, 500); // مدت زمان انیمیشن
            }
        }
    });

    // بستن منو هنگام کلیک روی دکمه Close
    closeBtn.addEventListener('click', (event) => {
        event.stopPropagation(); // جلوگیری از بسته شدن سریع
        navMenu.classList.remove('active');
        overlay.style.display = 'none';
        setTimeout(() => {
            navMenu.style.display = 'none';
        }, 500);
    });

    // آکاردئون برای لیست‌های کشویی
    $(document).ready(function () {
        $(".flip").click(function () {
            $(this).next('.panel').slideToggle("fast");
        });
    });
};
