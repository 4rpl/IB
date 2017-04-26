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