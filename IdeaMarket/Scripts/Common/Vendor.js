// набор скриптов для страниц продавца

var Vendor = Vendor || {}

Vendor.sidebar = function (curPage) {
    let lis = $('.sidebar li');
    for (let i = 0; i < lis.length; i++) {
        let li = $(lis[i]);
        if (li.find(`a[href='${curPage}']`).length > 0) {
            li.addClass('active');
            return;
        }
    }
}

Vendor.calculateWorth = function () {
    let words = $('#FullDescription').val().split(' ').length;
    if (words >= 5000) {
        $('.worthTip').text(words + ' слов, большой объем. Ценовой диапазон не определён');
        
    } else if (words >= 2000) {
        $('.worthTip').text(words + ' слов, средний объем. Рекомендуемая цена - до 50000 руб.');
    } else {
        $('.worthTip').text(words + ' слов, малый объем. Рекомендуемая цена - до 5000 руб.');
    }
}

Vendor.checkPassword = function () {
    let newPassword = $('#NewPassword');
    let confirmation = $('#Confirm');
    let passCheck = function () {
        if (newPassword.val().length < 6) {
            $('#NewPasswordAlert').text('Пароль слишком короткий (минимум 6 символов)');
            return;
        }
        $('#NewPasswordAlert').text('');
        if (newPassword.val() !== confirmation.val()) {
            $('#ConfirmAlert').text('Введённые пароли не совпадают');
            return;
        }
        $('#ConfirmAlert').text('');
    }
    newPassword.on('input', passCheck);
    confirmation.on('input', passCheck);
}