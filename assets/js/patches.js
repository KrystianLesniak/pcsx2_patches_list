// Search, filters and sorting for the patches table generated into markdown_patches.md.
(function () {
  var table = document.querySelector('.post-content table');
  if (!table || !table.tHead || !table.tBodies[0]) return;

  var REGIONS = [
    ['NTSC-U', /^S[CL]US/],
    ['PAL', /^(S[CL]E[SD]|TCES)/],
    ['NTSC-J', /^(S[CL]P[SM]|P[ABC]PX)/],
    ['NTSC-K', /^S[CL]KA/],
    ['Asia', /^S[CL]AJ/]
  ];
  // shortcut: keyword match on free-text comments, misses patches described in other words
  var TYPES = [
    ['Widescreen', /widescreen|16:9|\bws\b/i],
    ['No-Interlace', /interlac/i],
    ['50/60 FPS', /fps/i]
  ];

  var tbody = table.tBodies[0];
  var headers = Array.prototype.map.call(table.tHead.rows[0].cells, function (th) { return th.textContent.trim(); });

  var rows = Array.prototype.map.call(tbody.rows, function (tr) {
    Array.prototype.forEach.call(tr.cells, function (td, i) {
      td.setAttribute('data-label', headers[i]);
      if (!td.textContent.trim()) td.textContent = ''; // "&nbsp;"-only cells, so CSS :empty hides them on phones
    });
    var code = tr.cells[0].textContent.trim();
    var comment = tr.cells[3] ? tr.cells[3].textContent : '';
    var row = {
      tr: tr,
      text: tr.textContent.toLowerCase(),
      keys: Array.prototype.map.call(tr.cells, function (td) { return td.textContent.trim(); }),
      region: (REGIONS.filter(function (r) { return r[1].test(code); })[0] || [null])[0],
      types: TYPES.filter(function (t) { return t[1].test(comment); }).map(function (t) { return t[0]; })
    };
    if (row.keys[2] && navigator.clipboard) tr.cells[2].appendChild(copyButton(row.keys[2]));
    return row;
  });

  function copyButton(crc) {
    var btn = document.createElement('button');
    btn.type = 'button';
    btn.className = 'patches-copy';
    btn.textContent = 'Copy';
    btn.setAttribute('aria-label', 'Copy CRC ' + crc);
    btn.addEventListener('click', function () {
      navigator.clipboard.writeText(crc).then(function () {
        btn.textContent = 'Copied';
        setTimeout(function () { btn.textContent = 'Copy'; }, 1500);
      });
    });
    return btn;
  }
  var order = rows.slice();

  var params = new URLSearchParams(location.search);
  var state = {
    q: params.get('q') || '',
    type: (params.get('type') || '').split(',').filter(Boolean),
    region: (params.get('region') || '').split(',').filter(Boolean),
    sort: -1,
    dir: 1
  };

  // Controls
  var controls = document.createElement('div');
  controls.className = 'patches-controls';
  var search = document.createElement('input');
  search.type = 'search';
  search.placeholder = 'Search title, game code, CRC or comment…  ( / )';
  search.setAttribute('aria-label', 'Search patches');
  search.value = state.q;
  controls.appendChild(search);
  controls.appendChild(chipGroup('Patch type', 'type', TYPES));
  controls.appendChild(chipGroup('Region', 'region', REGIONS));
  var status = document.createElement('p');
  status.className = 'patches-count';
  var count = document.createElement('span');
  count.setAttribute('aria-live', 'polite');
  var clear = document.createElement('button');
  clear.type = 'button';
  clear.className = 'patches-clear';
  clear.textContent = 'Clear filters';
  clear.addEventListener('click', function () {
    state.q = search.value = '';
    state.type = [];
    state.region = [];
    controls.querySelectorAll('[aria-pressed]').forEach(function (b) { b.setAttribute('aria-pressed', 'false'); });
    render();
    search.focus();
  });
  status.appendChild(count);
  status.appendChild(clear);
  controls.appendChild(status);
  table.parentNode.insertBefore(controls, table);
  table.classList.add('patches-table');

  function chipGroup(label, key, items) {
    var group = document.createElement('div');
    group.className = 'patches-chips';
    group.setAttribute('role', 'group');
    group.setAttribute('aria-label', label);
    var title = document.createElement('span');
    title.textContent = label + ':';
    group.appendChild(title);
    items.forEach(function (item) {
      var name = item[0];
      var btn = document.createElement('button');
      btn.type = 'button';
      btn.textContent = name;
      btn.setAttribute('aria-pressed', String(state[key].indexOf(name) >= 0));
      btn.addEventListener('click', function () {
        var i = state[key].indexOf(name);
        if (i >= 0) state[key].splice(i, 1); else state[key].push(name);
        btn.setAttribute('aria-pressed', String(i < 0));
        render();
      });
      group.appendChild(btn);
    });
    return group;
  }

  // Sortable headers
  Array.prototype.forEach.call(table.tHead.rows[0].cells, function (th, col) {
    var btn = document.createElement('button');
    btn.type = 'button';
    btn.textContent = headers[col];
    btn.addEventListener('click', function () {
      state.dir = state.sort === col ? -state.dir : 1;
      state.sort = col;
      Array.prototype.forEach.call(table.tHead.rows[0].cells, function (h) { h.removeAttribute('aria-sort'); });
      th.setAttribute('aria-sort', state.dir > 0 ? 'ascending' : 'descending');
      // no numeric collation: CRCs are hex, so "83BB" must not sort as the number 83
      var collator = new Intl.Collator(undefined, { sensitivity: 'base' });
      order = rows.slice().sort(function (a, b) {
        var x = a.keys[col], y = b.keys[col];
        if (!x || !y) return x ? -1 : y ? 1 : 0; // empty cells always last
        return state.dir * collator.compare(x, y);
      });
      render();
    });
    th.textContent = '';
    th.appendChild(btn);
  });

  function matches(row, terms) {
    return terms.every(function (t) { return row.text.indexOf(t) >= 0; }) &&
      (!state.type.length || state.type.some(function (t) { return row.types.indexOf(t) >= 0; })) &&
      (!state.region.length || state.region.indexOf(row.region) >= 0);
  }

  // Re-append only matching rows so zebra striping (nth-child) stays correct.
  function render() {
    var terms = state.q.toLowerCase().split(/\s+/).filter(Boolean);
    var frag = document.createDocumentFragment();
    var shown = 0;
    order.forEach(function (row) {
      if (matches(row, terms)) { frag.appendChild(row.tr); shown++; }
    });
    tbody.textContent = '';
    tbody.appendChild(frag);
    count.textContent = shown ? 'Showing ' + shown + ' of ' + rows.length + ' patches' : 'No patches found';
    clear.hidden = !(state.q || state.type.length || state.region.length);

    var p = new URLSearchParams();
    if (state.q) p.set('q', state.q);
    if (state.type.length) p.set('type', state.type.join(','));
    if (state.region.length) p.set('region', state.region.join(','));
    var qs = p.toString();
    history.replaceState(null, '', location.pathname + (qs ? '?' + qs : '') + location.hash);
  }

  var timer;
  search.addEventListener('input', function () {
    clearTimeout(timer);
    timer = setTimeout(function () { state.q = search.value.trim(); render(); }, 120);
  });

  // "/" jumps to the search box, like on GitHub
  document.addEventListener('keydown', function (e) {
    var tag = document.activeElement && document.activeElement.tagName;
    if (e.key === '/' && tag !== 'INPUT' && tag !== 'TEXTAREA') {
      e.preventDefault();
      search.focus();
    }
  });

  render();
})();
