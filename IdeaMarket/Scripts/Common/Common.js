// набор общих скриптов сайта

var Project = Project || {}
Project.Common = Project.Common || {}

Project.Common.sortTable = function (table, col) {
    let span = $(table.find("thead tr th")[col]).find("a span");
    let reverse = span.attr('class') === "glyphicon glyphicon-sort-by-alphabet";

    table.find("thead tr th span").remove();

    if (reverse) {
        $(table.find("thead tr th").eq(col).find('a')).append(' <span class="glyphicon glyphicon-sort-by-alphabet-alt"></span>');
    } else {
        $(table.find("thead tr th").eq(col).find('a')).append(' <span class="glyphicon glyphicon-sort-by-alphabet"></span>');
    }

    let tb = table.find("tbody")[0]; // use `<tbody>` to ignore `<thead>` and `<tfoot>` rows
    if (!tb) {
        return;
    }
    let tr = Array.prototype.slice.call(tb.rows, 0); // put rows into array
    let i;
    reverse = -((+reverse) || -1);
    tr = tr.sort(function (a, b) { // sort rows
        return reverse // `-1 *` if want opposite order
            * (a.cells[col].textContent.trim() // using `.textContent.trim()` for test
                .localeCompare(b.cells[col].textContent.trim())
               );
    });
    for (i = 0; i < tr.length; ++i) {
        tb.appendChild(tr[i]);
    }// append each row in order
}