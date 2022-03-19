import * as alt from 'alt-client';
import * as native from 'natives';

const activateInterior = (id, interiors) => {
  interiors.forEach((interior) => {
    if (!native.isInteriorEntitySetActive(id, interior.name)) {
      native.activateInteriorEntitySet(id, interior.name);
      if (interior.color) {
        native.setInteriorEntitySetColor(id, interior.name, interior.color);
      }
    }
  })
};


alt.onServer('LOADING MRPD', () => {
  
  let gabzmrpd = native.getInteriorAtCoords(451.0129, -993.3741, 29.1718);
  native.pinInteriorInMemory(gabzmrpd);
  if (native.isValidInterior(gabzmrpd)) {
    const data = [
      { name: "branded_style_set" },
      { name: "v_gabz_mrpd_rm1" },
      { name: "v_gabz_mrpd_rm2" },
      { name: "v_gabz_mrpd_rm3" },
      { name: "v_gabz_mrpd_rm4" },
      { name: "v_gabz_mrpd_rm5" },
      { name: "v_gabz_mrpd_rm6" },
      { name: "v_gabz_mrpd_rm7" },
      { name: "v_gabz_mrpd_rm8" },
      { name: "v_gabz_mrpd_rm9" },
      { name: "v_gabz_mrpd_rm10" },
      { name: "v_gabz_mrpd_rm11" },
      { name: "v_gabz_mrpd_rm12" },
      { name: "v_gabz_mrpd_rm13" },
      { name: "v_gabz_mrpd_rm14" },
      { name: "v_gabz_mrpd_rm15" },
      { name: "v_gabz_mrpd_rm16" },
      { name: "v_gabz_mrpd_rm17" },
      { name: "v_gabz_mrpd_rm18" },
      { name: "v_gabz_mrpd_rm19" },
      { name: "v_gabz_mrpd_rm20" },
      { name: "v_gabz_mrpd_rm21" },
      { name: "v_gabz_mrpd_rm22" },
      { name: "v_gabz_mrpd_rm23" },
      { name: "v_gabz_mrpd_rm24" },
      { name: "v_gabz_mrpd_rm25" },
      { name: "v_gabz_mrpd_rm26" },
      { name: "v_gabz_mrpd_rm27" },
      { name: "v_gabz_mrpd_rm28" },
      { name: "v_gabz_mrpd_rm29" },
      { name: "v_gabz_mrpd_rm30" },
      { name: "v_gabz_mrpd_rm31" },
      
    ];
    activateInterior(gabzmrpd, data);
    native.refreshInterior(gabzmrpd)
  };


});

