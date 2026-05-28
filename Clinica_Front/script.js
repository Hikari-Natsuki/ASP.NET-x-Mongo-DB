// URL base para conectar el frontend con el backend local en .NET
const BASE_URL = "http://localhost:5244";

// Almacenamiento temporal en memoria para los arrays de los formularios
// Evita que los datos dinámicos (listas) se borren antes de enviar la petición POST/PUT
let arraysFormularios = {
    psi: { antecedentes: [], enfermedad: [], tratamiento: [], recomendaciones: [] },
    fis: { antecedentes: [], arcos: [], fuerza: [], tratamiento: [], recomendaciones: [] },
    odo: { antecedentes: [], odontograma: [], tratamiento: [], recomendaciones: [] }
};

// Almacén global (caché) para guardar los resultados de las búsquedas por número de documento
// Permite seleccionar un elemento de la tabla y cargarlo para edición sin volver a consultar al servidor
let cacheBusquedas = {
    psi: [],
    fis: [],
    odo: []
};

// Navegación SPA (Single Page Application)
// Oculta las secciones inactivas y muestra únicamente la pestaña seleccionada
function switchTab(tabId) {
    // Remueve la clase 'active' de todos los contenidos de pestaña y enlaces de navegación
    document.querySelectorAll('.tab-content').forEach(tab => tab.classList.remove('active'));
    document.querySelectorAll('.nav-link').forEach(link => link.classList.remove('active'));
    
    // Muestra el contenedor correspondiente a la pestaña clickeada
    const targetTab = document.getElementById(`tab-${tabId}`);
    if (targetTab) targetTab.classList.add('active');
    
    // Ilumina el botón del menú que disparó el evento
    if (window.event && window.event.currentTarget) {
        window.event.currentTarget.classList.add('active');
    }
}

// =========================================================================
// --- MANEJO DE ARRAYS DINÁMICOS POR MÓDULO ---
// =========================================================================

// 1. Agregar Antecedente (Función compartida por Psicología, Fisioterapia y Odontología)
function addAntecedente(mod) {
    // Obtiene los inputs dinámicos dependiendo del prefijo del módulo ('psi', 'fis' o 'odo')
    const tipo = document.getElementById(`input-${mod}-antecedentes-tipo`);
    const obs = document.getElementById(`input-${mod}-antecedentes-obs`);
    
    // Validación de campos obligatorios
    if(!tipo || !obs || !tipo.value || !obs.value) return showToast("Completa ambos campos del antecedente", "error");

    // Agrega el objeto antecedente al array global correspondiente en memoria
    arraysFormularios[mod].antecedentes.push({ tipo: tipo.value, observaciones: obs.value });
    
    // Renderiza visualmente el nuevo ítem en la lista HTML de la interfaz
    renderList(mod, 'antecedentes', `${tipo.value} - ${obs.value}`);
    
    // Limpia los campos de texto para permitir una nueva inserción
    tipo.value = ""; obs.value = "";
}

// 2. Agregar ítem simple de texto (Para arrays planos de strings como enfermedadActual, tratamiento y recomendaciones)
function addSimpleItem(mod, campo) {
    const input = document.getElementById(`input-${mod}-${campo}`);
    if(!input || !input.value) return showToast("El campo no puede estar vacío", "error");

    // Inserta la cadena de texto directamente en el array del módulo y campo correspondiente
    arraysFormularios[mod][campo].push(input.value);
    
    // Lo pinta en la interfaz de usuario
    renderList(mod, campo, input.value);
    
    // Vacía el input de texto
    input.value = "";
}

// 3. Fisioterapia: Captura de Arcos de Movilidad
function addArcoMovilidad() {
    const art = document.getElementById("input-fis-arcos-art");
    const mov = document.getElementById("input-fis-arcos-mov");
    const grados = document.getElementById("input-fis-arcos-grados");
    if(!art || !mov || !grados || !art.value || !mov.value || !grados.value) return showToast("Completa los datos del arco", "error");

    // Empuja un objeto estructurado al array de arcos de movilidad, convirtiendo los grados a un entero
    arraysFormularios.fis.arcos.push({ articulacion: art.value, movimiento: mov.value, gradosObtenidos: parseInt(grados.value) });
    
    // Muestra la información formateada con el símbolo de grados (°)
    renderList('fis', 'arcos', `${art.value} (${mov.value}): ${grados.value}°`);
    
    // Resetea los tres campos del flujo de arcos
    art.value = ""; mov.value = ""; grados.value = "";
}

// 4. Fisioterapia: Captura de Fuerza Muscular
function addFuerzaMuscular() {
    const mus = document.getElementById("input-fis-fuerza-mus");
    const grado = document.getElementById("input-fis-fuerza-grado");
    if(!mus || !grado || !mus.value || !grado.value) return showToast("Completa los datos de fuerza", "error");

    // Inserta el músculo y su valor correspondiente transformado a número
    arraysFormularios.fis.fuerza.push({ musculo: mus.value, gradoFuerza: parseInt(grado.value) });
    
    // Muestra el formato visual en el listado
    renderList('fis', 'fuerza', `${mus.value} -> Escala: ${grado.value}`);
    
    // Limpia los campos de fuerza
    mus.value = ""; grado.value = "";
}

// 5. Odontología: Registro de hallazgos en el Odontograma
function addOdontograma() {
    const diente = document.getElementById("input-odo-odontograma-diente");
    const cara = document.getElementById("input-odo-odontograma-cara");
    const estado = document.getElementById("input-odo-odontograma-estado");
    const obs = document.getElementById("input-odo-odontograma-obs");
    
    if(!diente || !cara || !estado || !diente.value || !cara.value || !estado.value) return showToast("Número, cara y estado son obligatorios", "error");

    // Inserta la pieza dental estructurada en el array de odontograma
    arraysFormularios.odo.odontograma.push({ numeroDiente: parseInt(diente.value), cara: cara.value, estado: estado.value, observaciones: obs.value });
    
    // Pinta el registro en la interfaz
    renderList('odo', 'odontograma', `Diente ${diente.value} [${cara.value}] - ${estado.value}`);
    
    // Limpia todos los selectores e inputs del odontograma
    diente.value = ""; cara.value = ""; estado.value = ""; obs.value = "";
}

// Renderizador visual genérico de las listas dinámicas en el DOM
function renderList(mod, campo, textoMostrar) {
    const listaHtml = document.getElementById(`list-${mod}-${campo}`);
    if (!listaHtml) return;
    
    // Obtiene el índice del último elemento añadido para asignárselo al botón de eliminación
    const index = arraysFormularios[mod][campo].length - 1;
    const li = document.createElement("li");
    
    // Inyecta el texto junto con un botón "X" configurado para ejecutar el borrado por índice
    li.innerHTML = `<span>${textoMostrar}</span><button type="button" class="btn-remove" onclick="removeElement('${mod}', '${campo}', ${index}, this)">X</button>`;
    listaHtml.appendChild(li);
}

// Elimina elementos de los arrays temporales y re-indexa la lista visual
function removeElement(mod, campo, index, elementoBtn) {
    // Elimina el elemento del array en memoria usando su índice de posición
    arraysFormularios[mod][campo].splice(index, 1);
    
    // Remueve físicamente el nodo <li> del árbol HTML de forma inmediata
    elementoBtn.parentElement.remove();
    
    // Re-renderizado total de la lista para actualizar los índices en los botones "X" restantes
    const listaHtml = document.getElementById(`list-${mod}-${campo}`);
    if(listaHtml) {
        listaHtml.innerHTML = ""; // Vacía la lista actual
        
        // Recorre el array modificado para volver a dibujar los elementos con sus nuevos índices correctos
        arraysFormularios[mod][campo].forEach((item, idx) => {
            let texto = typeof item === 'string' ? item : (item.tipo ? `${item.tipo} - ${item.observaciones}` : JSON.stringify(item));
            
            // Evaluaciones de formato personalizado según el tipo de datos del campo
            if(campo === 'arcos') texto = `${item.articulacion} (${item.movimiento}): ${item.gradosObtenidos}°`;
            if(campo === 'fuerza') texto = `${item.musculo} -> Escala: ${item.gradoFuerza}`;
            if(campo === 'odontograma') texto = `Diente ${item.numeroDiente} [${item.cara}] - ${item.estado}`;
            
            const li = document.createElement("li");
            li.innerHTML = `<span>${texto}</span><button type="button" class="btn-remove" onclick="removeElement('${mod}', '${campo}', ${idx}, this)">X</button>`;
            listaHtml.appendChild(li);
        });
    }
}

// Limpieza de todos los estados en memoria e inputs de texto tras realizar un registro exitoso
function resetFormularios(mod) {
    // Mapea el prefijo corto al ID real del formulario HTML
    const formId = mod === 'psi' ? 'psicologia' : mod === 'fis' ? 'fisioterapia' : 'odontologia';
    const formulario = document.getElementById(`form-${formId}`);
    if (formulario) formulario.reset(); // Resetea campos nativos de texto, número y selects

    // Recorre todas las listas del módulo para vaciar los arrays globales e interfaces gráficas
    Object.keys(arraysFormularios[mod]).forEach(key => {
        arraysFormularios[mod][key] = [];
        const ul = document.getElementById(`list-${mod}-${key}`);
        if(ul) ul.innerHTML = "";
    });
}

// =========================================================================
// --- BUSCAR HISTORIALES MULTI-MÓDULO ---
// =========================================================================
async function buscarHistoriales(modulo) {
    const inputDoc = document.getElementById(`${modulo}-search-doc`);
    const numDoc = inputDoc ? inputDoc.value : "";
    
    if (!numDoc) return showToast("Por favor, ingrese un número de documento.", "error");

    // Determina el segmento de la URL según la especialidad médica
    let segment = modulo === 'psi' ? 'psicologia' : modulo === 'fis' ? 'fisioterapia' : 'odontologia';

    try {
        // Realiza la petición GET al endpoint de búsqueda del backend de .NET
        const response = await fetch(`${BASE_URL}/${segment}/buscar/${numDoc}`);
        if (!response.ok) throw new Error("No se encontraron registros para este paciente.");

        // Guarda el arreglo de historiales clínicos encontrados en la caché global
        cacheBusquedas[modulo] = await response.json(); 
        
        // Elementos de la tabla de resultados en el DOM
        const tbody = document.getElementById(`tabla-resultados-${modulo}`);
        const contenedor = document.getElementById(`contenedor-tabla-${modulo}`);
        const resultDisplay = document.getElementById(`result-${modulo}`);
        
        tbody.innerHTML = ""; 
        if (resultDisplay) resultDisplay.innerHTML = "";

        // Si el array llega vacío, oculta la tabla y muestra un aviso en pantalla
        if (cacheBusquedas[modulo].length === 0) {
            if (contenedor) contenedor.style.display = "none";
            if (resultDisplay) resultDisplay.innerHTML = "<p>No se encontraron historiales clínicos.</p>";
            return;
        }

        // Itera sobre cada consulta clínica para inyectarla en las filas (<tr>) de la tabla
        cacheBusquedas[modulo].forEach((historia, index) => {
            const fila = document.createElement("tr");
            
            // Al hacer clic en cualquier parte de la fila, se ejecuta el autocompletado del formulario
            fila.onclick = () => cargarDatosEnFormulario(modulo, index);
            
            // Formatear la fecha ISO proveniente de MongoDB para que se vea legible (dd/mm/aaaa)
            let fechaConsulta = "Sin fecha";
            if(historia.fecha) {
                fechaConsulta = new Date(historia.fecha).toLocaleDateString('es-ES', {
                    day: '2-digit', month: '2-digit', year: 'numeric'
                });
            }

            // Inyecta el HTML de las celdas asignando el documento, el nombre y la insignia de fecha
            fila.innerHTML = `
                <td><b>${historia.paciente.documento.numero}</b></td>
                <td>${historia.paciente.nombre}</td>
                <td><span class="badge" style="background-color: #17a2b8; color: white; padding: 4px 8px; border-radius: 4px;">${fechaConsulta}</span></td>
                <td><button class="btn btn-sm btn-primary">Editar</button></td>
            `;
            tbody.appendChild(fila);
        });

        // Hace visible la tabla de resultados
        if (contenedor) contenedor.style.display = "block";
        showToast("Historiales clínicos cargados.", "success");

    } catch (error) {
        // Manejo de errores visuales en caso de falla de red o registros inexistentes
        if (document.getElementById(`contenedor-tabla-${modulo}`)) {
            document.getElementById(`contenedor-tabla-${modulo}`).style.display = "none";
        }
        if (document.getElementById(`result-${modulo}`)) {
            document.getElementById(`result-${modulo}`).innerHTML = `<p style="color:red;">${error.message}</p>`;
        }
    }
}

// =========================================================================
// --- AUTOCOMPLETADO PARA EDICIÓN ---
// =========================================================================
function cargarDatosEnFormulario(modulo, index) {
    // Recupera la consulta seleccionada desde la caché local usando su índice de fila
    const historia = cacheBusquedas[modulo][index];
    if (!historia) return;

    resetFormularios(modulo); // Resetea estados e interfaces previas antes de rellenar

    // --- BLOQUE PSICOLOGÍA ---
    if (modulo === 'psi') {
        document.getElementById("psi-id").value = historia.id || "";
        document.getElementById("psi-medico-id").value = historia.medicoId || "";
        document.getElementById("psi-entidad").value = historia.entidad || "";
        document.getElementById("psi-motivo").value = historia.motivoConsulta || "";
        document.getElementById("psi-examen").value = historia.examenMental || "";
        
        document.getElementById("psi-pac-nombre").value = historia.paciente.nombre || "";
        document.getElementById("psi-pac-edad").value = historia.paciente.edad || "";
        document.getElementById("psi-pac-sexo").value = historia.paciente.sexo || "";
        document.getElementById("psi-pac-tipo-doc").value = historia.paciente.documento.tipo || "";
        document.getElementById("psi-pac-doc").value = historia.paciente.documento.numero || "";

        // Restauración y re-pintado de sub-listas de Psicología
        if(historia.antecedentes) historia.antecedentes.forEach(a => { arraysFormularios.psi.antecedentes.push(a); renderList('psi','antecedentes', `${a.tipo} - ${a.observaciones}`); });
        if(historia.enfermedadActual) historia.enfermedadActual.forEach(e => { arraysFormularios.psi.enfermedad.push(e); renderList('psi','enfermedad', e); });
        if(historia.tratamiento) historia.tratamiento.forEach(t => { arraysFormularios.psi.tratamiento.push(t); renderList('psi','tratamiento', t); });
        if(historia.recomendaciones) historia.recomendaciones.forEach(r => { arraysFormularios.psi.recomendaciones.push(r); renderList('psi','recomendaciones', r); });
    }
    // --- BLOQUE FISIOTERAPIA ---
    else if (modulo === 'fis') {
        document.getElementById("fis-id").value = historia.id || "";
        document.getElementById("fis-medico-id").value = historia.medicoId || "";
        document.getElementById("fis-especialidad").value = historia.especialidad || "";
        document.getElementById("fis-entidad").value = historia.entidad || "";
        document.getElementById("fis-motivo").value = historia.motivoConsulta || "";
        document.getElementById("fis-postural").value = historia.evaluacionPostural || "";
        document.getElementById("fis-pruebas").value = historia.pruebasEspeciales || "";
        document.getElementById("fis-diagnostico").value = historia.diagnosticoFuncional || "";
        
        document.getElementById("fis-pac-nombre").value = historia.paciente.nombre || "";
        document.getElementById("fis-pac-edad").value = historia.paciente.edad || "";
        document.getElementById("fis-pac-sexo").value = historia.paciente.sexo || "";
        document.getElementById("fis-pac-tipo-doc").value = historia.paciente.documento.tipo || "";
        document.getElementById("fis-pac-doc").value = historia.paciente.documento.numero || "";

        // Restauración y re-pintado de sub-listas complejas de Fisioterapia
        if(historia.antecedentes) historia.antecedentes.forEach(a => { arraysFormularios.fis.antecedentes.push(a); renderList('fis','antecedentes', `${a.tipo} - ${a.observaciones}`); });
        if(historia.arcosMovilidad) historia.arcosMovilidad.forEach(arco => { arraysFormularios.fis.arcos.push(arco); renderList('fis','arcos', `${arco.articulacion} (${arco.movimiento}): ${arco.gradosObtenidos}°`); });
        if(historia.fuerzaMuscular) historia.fuerzaMuscular.forEach(f => { arraysFormularios.fis.fuerza.push(f); renderList('fis','fuerza', `${f.musculo} -> Escala: ${f.gradoFuerza}`); });
        if(historia.tratamiento) historia.tratamiento.forEach(t => { arraysFormularios.fis.tratamiento.push(t); renderList('fis','tratamiento', t); });
        if(historia.recomendaciones) historia.recomendaciones.forEach(r => { arraysFormularios.fis.recomendaciones.push(r); renderList('fis','recomendaciones', r); });
    }
    // --- BLOQUE ODONTOLOGÍA ---
    else if (modulo === 'odo') {
        document.getElementById("odo-id").value = historia.id || "";
        document.getElementById("odo-medico-id").value = historia.medicoId || "";
        document.getElementById("odo-especialidad").value = historia.especialidad || "";
        document.getElementById("odo-entidad").value = historia.entidad || "";
        document.getElementById("odo-higiene").value = historia.higieneOral || "";
        document.getElementById("odo-encias").value = historia.estadoEncias || "";
        document.getElementById("odo-motivo").value = historia.motivoConsulta || "";
        
        document.getElementById("odo-pac-nombre").value = historia.paciente.nombre || "";
        document.getElementById("odo-pac-edad").value = historia.paciente.edad || "";
        document.getElementById("odo-pac-sexo").value = historia.paciente.sexo || "";
        document.getElementById("odo-pac-tipo-doc").value = historia.paciente.documento.tipo || "";
        document.getElementById("odo-pac-doc").value = historia.paciente.documento.numero || "";

        // Restauración y re-pintado de sub-listas de Odontología (incluyendo odontograma)
        if(historia.antecedentes) historia.antecedentes.forEach(a => { arraysFormularios.odo.antecedentes.push(a); renderList('odo','antecedentes', `${a.tipo} - ${a.observaciones}`); });
        if(historia.odontograma) historia.odontograma.forEach(o => { arraysFormularios.odo.odontograma.push(o); renderList('odo','odontograma', `Diente ${o.numeroDiente} [${o.cara}] - ${o.estado}`); });
        if(historia.tratamiento) historia.tratamiento.forEach(t => { arraysFormularios.odo.tratamiento.push(t); renderList('odo','tratamiento', t); });
        if(historia.recomendaciones) historia.recomendaciones.forEach(r => { arraysFormularios.odo.recomendaciones.push(r); renderList('odo','recomendaciones', r); });
    }

    // Efecto visual: Desplaza suavemente el scroll hacia arriba para empezar a editar de inmediato
    window.scrollTo({ top: 0, behavior: 'smooth' });
    showToast(`Historial cargado en el formulario.`, "info");
}

// =========================================================================
// --- OPERACIONES HTTP CON EL BACKEND (POST, PUT, DELETE, GET) ---
// =========================================================================
async function executeAction(module, action) {
    let url = "";
    // Configuración por defecto para las peticiones HTTP
    let options = { method: "GET", headers: { "Content-Type": "application/json" } };
    let payload = {};

    try {
        // --- PROCESAMIENTO DEL MÓDULO PERSONAL MÉDICO ---
        if (module === 'doc') {
            if (action === 'insert') {
                payload = {
                    id: document.getElementById("doc-id").value,
                    nombre: document.getElementById("doc-nombre").value,
                    apellido: document.getElementById("doc-apellido").value,
                    especialidad: document.getElementById("doc-especialidad").value
                };
                url = `${BASE_URL}/personalmedico/registrar`;
                options.method = "POST";
                options.body = JSON.stringify(payload);
            } else if (action === 'search') {
                let id = document.getElementById("doc-search-id").value;
                if(!id) return showToast("El ID es obligatorio", "error");
                url = `${BASE_URL}/personalmedico/buscar/${id}`;
            }
        }
        // --- PROCESAMIENTO DEL MÓDULO PSICOLOGÍA ---
        else if (module === 'psi') {
            url = `${BASE_URL}/psicologia`;
            if (action === 'insert' || action === 'update') {
                // Empaqueta el modelo de datos JSON para enviar al backend
                payload = {
                    medicoId: document.getElementById("psi-medico-id").value,
                    entidad: document.getElementById("psi-entidad").value,
                    motivoConsulta: document.getElementById("psi-motivo").value,
                    examenMental: document.getElementById("psi-examen").value,
                    paciente: {
                        nombre: document.getElementById("psi-pac-nombre").value,
                        edad: parseInt(document.getElementById("psi-pac-edad").value || 0),
                        sexo: document.getElementById("psi-pac-sexo").value,
                        documento: { 
                            tipo: document.getElementById("psi-pac-tipo-doc").value, 
                            numero: document.getElementById("psi-pac-doc").value 
                        }
                    },
                    // Vincula los arrays acumulados dinámicamente en memoria
                    antecedentes: arraysFormularios.psi.antecedentes,
                    enfermedadActual: arraysFormularios.psi.enfermedad,
                    tratamiento: arraysFormularios.psi.tratamiento,
                    recomendaciones: arraysFormularios.psi.recomendaciones
                };

                if (action === 'insert') {
                    url += "/registrar"; options.method = "POST"; options.body = JSON.stringify(payload);
                } else {
                    let id = document.getElementById("psi-id").value;
                    if(!id) return showToast("Se requiere el ID de la consulta para actualizar", "error");
                    url += `/actualizar/${id}`; options.method = "PUT"; options.body = JSON.stringify(payload);
                }
            } 
            else if (action === 'delete') { url += `/eliminar/${document.getElementById("psi-delete-id").value}`; options.method = "DELETE"; }
        }
        // --- PROCESAMIENTO DEL MÓDULO FISIOTERAPIA ---
        else if (module === 'fis') {
            url = `${BASE_URL}/fisioterapia`;
            if (action === 'insert' || action === 'update') {
                payload = {
                    medicoId: document.getElementById("fis-medico-id").value,
                    especialidad: document.getElementById("fis-especialidad").value,
                    entidad: document.getElementById("fis-entidad").value,
                    motivoConsulta: document.getElementById("fis-motivo").value,
                    evaluacionPostural: document.getElementById("fis-postural").value,
                    pruebasEspeciales: document.getElementById("fis-pruebas").value,
                    diagnosticoFuncional: document.getElementById("fis-diagnostico").value,
                    paciente: {
                        nombre: document.getElementById("fis-pac-nombre").value,
                        edad: parseInt(document.getElementById("fis-pac-edad").value || 0),
                        sexo: document.getElementById("fis-pac-sexo").value,
                        documento: { 
                            tipo: document.getElementById("fis-pac-tipo-doc").value, 
                            numero: document.getElementById("fis-pac-doc").value 
                        }
                    },
                    antecedentes: arraysFormularios.fis.antecedentes,
                    arcosMovilidad: arraysFormularios.fis.arcos,
                    fuerzaMuscular: arraysFormularios.fis.fuerza,
                    tratamiento: arraysFormularios.fis.tratamiento,
                    recomendaciones: arraysFormularios.fis.recomendaciones
                };

                if (action === 'insert') {
                    url += "/registrar"; options.method = "POST"; options.body = JSON.stringify(payload);
                } else {
                    let id = document.getElementById("fis-id").value;
                    if(!id) return showToast("Se requiere el ID para actualizar", "error");
                    url += `/actualizar/${id}`; options.method = "PUT"; options.body = JSON.stringify(payload);
                }
            } 
            else if (action === 'delete') 
                { url += `/eliminar/${document.getElementById("fis-delete-id").value}`; options.method = "DELETE"; }
        }
        // --- PROCESAMIENTO DEL MÓDULO ODONTOLOGÍA ---
        else if (module === 'odo') {
            url = `${BASE_URL}/odontologia`;
            if (action === 'insert' || action === 'update') {
                payload = {
                    medicoId: document.getElementById("odo-medico-id").value,
                    especialidad: document.getElementById("odo-especialidad").value,
                    entidad: document.getElementById("odo-entidad").value,
                    higieneOral: document.getElementById("odo-higiene").value,
                    estadoEncias: document.getElementById("odo-encias").value,
                    motivoConsulta: document.getElementById("odo-motivo").value,
                    paciente: {
                        nombre: document.getElementById("odo-pac-nombre").value,
                        edad: parseInt(document.getElementById("odo-pac-edad").value || 0),
                        sexo: document.getElementById("odo-pac-sexo").value,
                        documento: { 
                            tipo: document.getElementById("odo-pac-tipo-doc").value, 
                            numero: document.getElementById("odo-pac-doc").value 
                        }
                    },
                    antecedentes: arraysFormularios.odo.antecedentes,
                    odontograma: arraysFormularios.odo.odontograma,
                    tratamiento: arraysFormularios.odo.tratamiento,
                    recomendaciones: arraysFormularios.odo.recomendaciones
                };

                if (action === 'insert') {
                    url += "/registrar"; options.method = "POST"; options.body = JSON.stringify(payload);
                } else {
                    let id = document.getElementById("odo-id").value;
                    if(!id) return showToast("Se requiere el ID para actualizar", "error");
                    url += `/actualizar/${id}`; options.method = "PUT"; options.body = JSON.stringify(payload);
                }
            } 
            else if (action === 'delete') { url += `/eliminar/${document.getElementById("odo-delete-id").value}`; options.method = "DELETE"; }
        }

        // Ejecuta de forma unificada la petición asíncrona FETCH a la API
        const res = await fetch(url, options);
        
        // Si el estado de respuesta HTTP no es exitoso (fuera del rango 200-299), captura y lanza el texto del error
        if(!res.ok) {
            const errTxt = await res.text();
            throw new Error(errTxt || "Error en la petición del servidor.");
        }

        // Convierte la respuesta exitosa del servidor a un objeto JSON usable
        const data = await res.json();
        
        // Control de flujos de mensajes y limpiezas finales tras la respuesta de la API
        if (action === 'insert' || action === 'update') {
            // Extrae la propiedad 'mensaje' dinámica configurada en los controladores de .NET para el showToast
            let mensajeExito = data.mensaje || `Operación realizada con éxito en módulo: ${module}`;
            showToast(mensajeExito, "success");
            resetFormularios(module); // Limpia campos listos para un nuevo paciente
        } else if (action === 'delete') {
            showToast("Registro eliminado correctamente", "success");
        } else if (action === 'search' && module === 'doc') {
            // Muestra el JSON devuelto del médico dentro de una etiqueta <pre> formateada
            document.getElementById("result-doc").innerHTML = `<pre>${JSON.stringify(data, null, 2)}</pre>`;
        }

    } catch (err) {
        // Dispara una notificación de tipo error (roja) si algo sale mal en el try
        showToast(err.message, "error");
    }
}

// =========================================================================
// --- UTILIDADES GLOBALES (Toasts de notificación flotante) ---
// =========================================================================
function showToast(message, type = "success") {
    const container = document.getElementById("toast-container");
    if(!container) return; // Si no existe el contenedor en el HTML, cancela la ejecución
    
    const toast = document.createElement("div");
    toast.className = `toast toast-${type}`; // Aplica clases CSS dinámicas basadas en el tipo (success, error, info)
    toast.innerText = message;
    
    container.appendChild(toast); // Inserta la notificación en el DOM
    
    // Desvanece y elimina físicamente la notificación de la pantalla tras 3.5 segundos
    setTimeout(() => { toast.remove(); }, 3500);
}

// Limpieza de todos los estados en memoria e inputs de texto
function resetFormularios(mod) {
    // Mapea el prefijo corto al ID real del formulario HTML
    const formId = mod === 'psi' ? 'psicologia' : mod === 'fis' ? 'fisioterapia' : 'odontologia';
    const formulario = document.getElementById(`form-${formId}`);
    if (formulario) formulario.reset(); // Resetea campos nativos (inputs, selects, etc.)

    // EXTRA: Limpia manualmente los campos ocultos de IDs para evitar que se quede en modo "edición"
    const idOculto = document.getElementById(`${mod}-id`);
    if (idOculto) idOculto.value = "";
    
    const medicoIdOculto = document.getElementById(`${mod}-medico-id`);
    if (medicoIdOculto) medicoIdOculto.value = "";

    // Recorre todas las listas del módulo para vaciar los arrays globales de JavaScript e interfaces gráficas
    Object.keys(arraysFormularios[mod]).forEach(key => {
        arraysFormularios[mod][key] = [];
        const ul = document.getElementById(`list-${mod}-${key}`);
        if(ul) ul.innerHTML = "";
    });

    showToast("Formulario vaciado y listo", "info");
}