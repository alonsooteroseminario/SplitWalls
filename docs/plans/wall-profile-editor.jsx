import { useState, useRef, useCallback, useEffect } from "react";

const MM_PER_FOOT = 304.8;

// Default wall config
const DEFAULT_WALL = {
  lengthMm: 8000,
  heightMm: 2440,
  panelWidthMm: 1220,
};

// Segment colors
const SEGMENT_COLORS = [
  "#3B82F6", "#10B981", "#F59E0B", "#EF4444", "#8B5CF6",
  "#EC4899", "#06B6D4", "#84CC16", "#F97316", "#6366F1",
];

const PROFILE_SHAPES = [
  { id: "standard", label: "Standard", icon: "▬" },
  { id: "U", label: "U-Shape", icon: "⊔" },
  { id: "L_left", label: "L-Left", icon: "⌐" },
  { id: "L_right", label: "L-Right", icon: "¬" },
  { id: "T", label: "T-Shape", icon: "⊤" },
  { id: "I", label: "I-Shape", icon: "I" },
  { id: "borde", label: "Borde", icon: "⌈" },
];

function WallProfileEditor() {
  const canvasRef = useRef(null);
  const [wall, setWall] = useState({ ...DEFAULT_WALL });
  const [splitPoints, setSplitPoints] = useState([]);
  const [segments, setSegments] = useState([]);
  const [selectedSegment, setSelectedSegment] = useState(null);
  const [hoveredSplit, setHoveredSplit] = useState(null);
  const [draggingSplit, setDraggingSplit] = useState(null);
  const [openings, setOpenings] = useState([]);
  const [addingOpening, setAddingOpening] = useState(false);
  const [openingStart, setOpeningStart] = useState(null);
  const [mode, setMode] = useState("split"); // split, opening, select
  const [showGrid, setShowGrid] = useState(true);
  const [snapToGrid, setSnapToGrid] = useState(true);
  const [gridSizeMm, setGridSizeMm] = useState(100);
  const [generatedSkill, setGeneratedSkill] = useState(null);
  const [wallLengthInput, setWallLengthInput] = useState("8000");
  const [wallHeightInput, setWallHeightInput] = useState("2440");
  const [panelWidthInput, setPanelWidthInput] = useState("1220");
  const [showSkillPanel, setShowSkillPanel] = useState(false);

  // Canvas dimensions
  const CANVAS_W = 900;
  const CANVAS_H = 400;
  const PADDING = 60;
  const DRAW_W = CANVAS_W - PADDING * 2;
  const DRAW_H = CANVAS_H - PADDING * 2;

  const scale = Math.min(DRAW_W / wall.lengthMm, DRAW_H / wall.heightMm);
  const wallDrawW = wall.lengthMm * scale;
  const wallDrawH = wall.heightMm * scale;
  const offsetX = PADDING + (DRAW_W - wallDrawW) / 2;
  const offsetY = PADDING + (DRAW_H - wallDrawH) / 2;

  // Convert mm to canvas px
  const mmToX = (mm) => offsetX + mm * scale;
  const mmToY = (mm) => offsetY + (wall.heightMm - mm) * scale;
  const xToMm = (x) => (x - offsetX) / scale;
  const yToMm = (y) => wall.heightMm - (y - offsetY) / scale;

  const snapMm = (mm) => {
    if (!snapToGrid) return Math.round(mm);
    return Math.round(mm / gridSizeMm) * gridSizeMm;
  };

  // Rebuild segments whenever splitPoints change
  useEffect(() => {
    const sorted = [...splitPoints].sort((a, b) => a - b);
    const pts = [0, ...sorted, wall.lengthMm];
    const newSegments = [];
    for (let i = 0; i < pts.length - 1; i++) {
      const existing = segments.find(
        (s) => Math.abs(s.startMm - pts[i]) < 1 && Math.abs(s.endMm - pts[i + 1]) < 1
      );
      newSegments.push({
        startMm: pts[i],
        endMm: pts[i + 1],
        widthMm: pts[i + 1] - pts[i],
        profile: existing?.profile || "standard",
        label: existing?.label || `Panel ${i + 1}`,
        color: SEGMENT_COLORS[i % SEGMENT_COLORS.length],
        fireRating: existing?.fireRating || "none",
      });
    }
    setSegments(newSegments);
    if (selectedSegment !== null && selectedSegment >= newSegments.length) {
      setSelectedSegment(null);
    }
  }, [splitPoints, wall.lengthMm]);

  // Auto-split by panel width
  const autoSplit = () => {
    const pw = wall.panelWidthMm;
    const pts = [];
    let x = pw;
    while (x < wall.lengthMm - 10) {
      pts.push(x);
      x += pw;
    }
    setSplitPoints(pts);
  };

  // Clear all
  const clearAll = () => {
    setSplitPoints([]);
    setOpenings([]);
    setSelectedSegment(null);
    setGeneratedSkill(null);
  };

  const handleCanvasClick = (e) => {
    const rect = canvasRef.current.getBoundingClientRect();
    const x = e.clientX - rect.left;
    const y = e.clientY - rect.top;
    const mm = xToMm(x);
    const mmY = yToMm(y);

    if (mode === "split") {
      if (mm > 10 && mm < wall.lengthMm - 10 && mmY > -50 && mmY < wall.heightMm + 50) {
        const snapped = snapMm(mm);
        // Don't add if too close to existing
        const tooClose = splitPoints.some((p) => Math.abs(p - snapped) < 20);
        if (!tooClose && snapped > 0 && snapped < wall.lengthMm) {
          setSplitPoints([...splitPoints, snapped]);
        }
      }
    } else if (mode === "opening") {
      if (mmY >= 0 && mmY <= wall.heightMm && mm >= 0 && mm <= wall.lengthMm) {
        if (!addingOpening) {
          setAddingOpening(true);
          setOpeningStart({ x: snapMm(mm), y: snapMm(mmY) });
        } else {
          const endX = snapMm(mm);
          const endY = snapMm(mmY);
          const ox = Math.min(openingStart.x, endX);
          const ow = Math.abs(endX - openingStart.x);
          const oy = Math.min(openingStart.y, endY);
          const oh = Math.abs(endY - openingStart.y);
          if (ow > 50 && oh > 50) {
            setOpenings([
              ...openings,
              { xMm: ox, yMm: oy, widthMm: ow, heightMm: oh, type: "window" },
            ]);
          }
          setAddingOpening(false);
          setOpeningStart(null);
        }
      }
    } else if (mode === "select") {
      // Find clicked segment
      if (mmY >= 0 && mmY <= wall.heightMm) {
        const idx = segments.findIndex((s) => mm >= s.startMm && mm <= s.endMm);
        setSelectedSegment(idx >= 0 ? idx : null);
      }
    }
  };

  const handleCanvasMouseMove = (e) => {
    const rect = canvasRef.current.getBoundingClientRect();
    const x = e.clientX - rect.left;
    const mm = xToMm(x);

    if (draggingSplit !== null) {
      const snapped = snapMm(mm);
      if (snapped > 10 && snapped < wall.lengthMm - 10) {
        const newPts = [...splitPoints];
        newPts[draggingSplit] = snapped;
        setSplitPoints(newPts);
      }
      return;
    }

    // Check if hovering near a split point
    const idx = splitPoints.findIndex((p) => Math.abs(mmToX(p) - x) < 8);
    setHoveredSplit(idx >= 0 ? idx : null);
  };

  const handleCanvasMouseDown = (e) => {
    if (mode === "split" && hoveredSplit !== null) {
      e.preventDefault();
      setDraggingSplit(hoveredSplit);
    }
  };

  const handleCanvasMouseUp = () => {
    setDraggingSplit(null);
  };

  const handleRightClick = (e) => {
    e.preventDefault();
    const rect = canvasRef.current.getBoundingClientRect();
    const x = e.clientX - rect.left;
    const idx = splitPoints.findIndex((p) => Math.abs(mmToX(p) - x) < 10);
    if (idx >= 0) {
      setSplitPoints(splitPoints.filter((_, i) => i !== idx));
    }
  };

  const removeOpening = (idx) => {
    setOpenings(openings.filter((_, i) => i !== idx));
  };

  const updateSegmentProp = (idx, key, value) => {
    setSegments((prev) => {
      const copy = [...prev];
      copy[idx] = { ...copy[idx], [key]: value };
      return copy;
    });
  };

  // Generate skill JSON
  const generateSkill = () => {
    const skill = {
      name: "Custom Wall Split Standard",
      version: "1.0",
      created: new Date().toISOString(),
      wall: {
        totalLengthMm: wall.lengthMm,
        heightMm: wall.heightMm,
        defaultPanelWidthMm: wall.panelWidthMm,
      },
      splitRule: {
        method: splitPoints.length > 0 ? "custom" : "uniform",
        panelWidthMm: wall.panelWidthMm,
        splitPointsMm: [...splitPoints].sort((a, b) => a - b),
        snapToGridMm: snapToGrid ? gridSizeMm : null,
      },
      segments: segments.map((s, i) => ({
        index: i,
        startMm: s.startMm,
        endMm: s.endMm,
        widthMm: s.widthMm,
        profile: s.profile,
        label: s.label,
        fireRating: s.fireRating,
      })),
      openings: openings.map((o, i) => ({
        index: i,
        xMm: o.xMm,
        yMm: o.yMm,
        widthMm: o.widthMm,
        heightMm: o.heightMm,
        type: o.type,
      })),
      execution: {
        separatorWidthMm: 4,
        disableWallJoins: true,
        startFromCorner: 1,
        handleOpenings: openings.length > 0 ? "split_around" : "none",
      },
    };
    setGeneratedSkill(skill);
    setShowSkillPanel(true);
  };

  const updateWall = () => {
    const l = parseInt(wallLengthInput) || 8000;
    const h = parseInt(wallHeightInput) || 2440;
    const p = parseInt(panelWidthInput) || 1220;
    setWall({ lengthMm: l, heightMm: h, panelWidthMm: p });
    setSplitPoints([]);
    setOpenings([]);
    setSelectedSegment(null);
  };

  // --- Rendering ---
  const renderGrid = () => {
    if (!showGrid) return null;
    const lines = [];
    for (let x = 0; x <= wall.lengthMm; x += gridSizeMm) {
      const px = mmToX(x);
      lines.push(
        <line key={`gv${x}`} x1={px} y1={offsetY} x2={px} y2={offsetY + wallDrawH}
          stroke="#1e293b" strokeWidth={0.5} opacity={0.15} />
      );
    }
    for (let y = 0; y <= wall.heightMm; y += gridSizeMm) {
      const py = mmToY(y);
      lines.push(
        <line key={`gh${y}`} x1={offsetX} y1={py} x2={offsetX + wallDrawW} y2={py}
          stroke="#1e293b" strokeWidth={0.5} opacity={0.15} />
      );
    }
    return lines;
  };

  const renderSegments = () =>
    segments.map((s, i) => {
      const x = mmToX(s.startMm);
      const w = s.widthMm * scale;
      const isSelected = selectedSegment === i;
      return (
        <g key={`seg${i}`}>
          <rect x={x + 1} y={offsetY + 1} width={Math.max(w - 2, 1)} height={wallDrawH - 2}
            fill={s.color} opacity={isSelected ? 0.35 : 0.15}
            stroke={isSelected ? s.color : "none"} strokeWidth={isSelected ? 2 : 0}
            rx={2}
            style={{ cursor: mode === "select" ? "pointer" : "default" }}
          />
          {w > 40 && (
            <text x={x + w / 2} y={offsetY + wallDrawH + 16}
              textAnchor="middle" fontSize={10} fill="#94a3b8" fontFamily="monospace">
              {s.widthMm}mm
            </text>
          )}
          {w > 60 && (
            <text x={x + w / 2} y={offsetY + wallDrawH / 2}
              textAnchor="middle" fontSize={9} fill={s.color} fontFamily="monospace"
              opacity={0.8} fontWeight={600}>
              {s.label}
            </text>
          )}
        </g>
      );
    });

  const renderSplitPoints = () =>
    splitPoints.map((p, i) => {
      const x = mmToX(p);
      const isHovered = hoveredSplit === i;
      return (
        <g key={`sp${i}`}>
          <line x1={x} y1={offsetY - 8} x2={x} y2={offsetY + wallDrawH + 8}
            stroke={isHovered ? "#f43f5e" : "#f97316"} strokeWidth={isHovered ? 2.5 : 1.5}
            strokeDasharray={isHovered ? "none" : "6,3"}
            style={{ cursor: "ew-resize" }}
          />
          <circle cx={x} cy={offsetY - 12} r={isHovered ? 5 : 4}
            fill={isHovered ? "#f43f5e" : "#f97316"} stroke="#0f172a" strokeWidth={1.5} />
          <text x={x} y={offsetY - 22} textAnchor="middle" fontSize={9}
            fill="#f97316" fontFamily="monospace" fontWeight={600}>
            {p}
          </text>
        </g>
      );
    });

  const renderOpenings = () =>
    openings.map((o, i) => {
      const x = mmToX(o.xMm);
      const y = mmToY(o.yMm + o.heightMm);
      const w = o.widthMm * scale;
      const h = o.heightMm * scale;
      return (
        <g key={`op${i}`}>
          <rect x={x} y={y} width={w} height={h}
            fill="#38bdf8" opacity={0.25} stroke="#38bdf8" strokeWidth={1.5}
            strokeDasharray="4,2" rx={3} />
          <text x={x + w / 2} y={y + h / 2 + 4}
            textAnchor="middle" fontSize={9} fill="#38bdf8" fontFamily="monospace">
            {o.type} {o.widthMm}×{o.heightMm}
          </text>
          <circle cx={x + w - 2} cy={y + 2} r={6} fill="#ef4444" opacity={0.7}
            style={{ cursor: "pointer" }}
            onClick={(e) => { e.stopPropagation(); removeOpening(i); }} />
          <text x={x + w - 2} y={y + 6} textAnchor="middle" fontSize={8} fill="white"
            style={{ pointerEvents: "none" }}>×</text>
        </g>
      );
    });

  const renderDimensions = () => (
    <g>
      {/* Total length */}
      <line x1={offsetX} y1={offsetY + wallDrawH + 35} x2={offsetX + wallDrawW} y2={offsetY + wallDrawH + 35}
        stroke="#64748b" strokeWidth={1} markerStart="url(#arrowL)" markerEnd="url(#arrowR)" />
      <text x={offsetX + wallDrawW / 2} y={offsetY + wallDrawH + 50}
        textAnchor="middle" fontSize={11} fill="#cbd5e1" fontFamily="monospace" fontWeight={700}>
        {wall.lengthMm} mm
      </text>
      {/* Height */}
      <line x1={offsetX - 25} y1={offsetY} x2={offsetX - 25} y2={offsetY + wallDrawH}
        stroke="#64748b" strokeWidth={1} markerStart="url(#arrowU)" markerEnd="url(#arrowD)" />
      <text x={offsetX - 30} y={offsetY + wallDrawH / 2}
        textAnchor="middle" fontSize={10} fill="#cbd5e1" fontFamily="monospace"
        transform={`rotate(-90, ${offsetX - 30}, ${offsetY + wallDrawH / 2})`}>
        {wall.heightMm} mm
      </text>
    </g>
  );

  return (
    <div style={{
      minHeight: "100vh",
      background: "#0a0e1a",
      color: "#e2e8f0",
      fontFamily: "'JetBrains Mono', 'SF Mono', 'Fira Code', monospace",
    }}>
      {/* Header */}
      <div style={{
        padding: "16px 24px",
        borderBottom: "1px solid #1e293b",
        display: "flex",
        alignItems: "center",
        gap: 16,
        background: "#0d1224",
      }}>
        <div style={{
          width: 36, height: 36, borderRadius: 8,
          background: "linear-gradient(135deg, #f97316, #ef4444)",
          display: "flex", alignItems: "center", justifyContent: "center",
          fontSize: 18, fontWeight: 900,
        }}>⫽</div>
        <div>
          <div style={{ fontSize: 16, fontWeight: 700, color: "#f8fafc", letterSpacing: "-0.02em" }}>
            Wall Profile Editor
          </div>
          <div style={{ fontSize: 11, color: "#64748b", marginTop: 1 }}>
            Define split points and segment profiles for your panelization standard
          </div>
        </div>
        <div style={{ flex: 1 }} />
        <div style={{
          fontSize: 10, color: "#475569", padding: "4px 10px",
          background: "#1e293b", borderRadius: 6,
        }}>
          {mode === "split" ? "Click to add splits · Drag to move · Right-click to remove" :
           mode === "opening" ? "Click two corners to define an opening" :
           "Click a segment to select and edit properties"}
        </div>
      </div>

      <div style={{ display: "flex", height: "calc(100vh - 69px)" }}>
        {/* Left panel - Controls */}
        <div style={{
          width: 260, borderRight: "1px solid #1e293b", padding: 16,
          overflowY: "auto", background: "#0d1224",
          display: "flex", flexDirection: "column", gap: 16,
        }}>
          {/* Wall dimensions */}
          <Section title="Wall Dimensions">
            <InputRow label="Length (mm)" value={wallLengthInput}
              onChange={(v) => setWallLengthInput(v)} />
            <InputRow label="Height (mm)" value={wallHeightInput}
              onChange={(v) => setWallHeightInput(v)} />
            <InputRow label="Panel W (mm)" value={panelWidthInput}
              onChange={(v) => setPanelWidthInput(v)} />
            <button onClick={updateWall} style={btnStyle("#334155")}>Apply Dimensions</button>
          </Section>

          {/* Mode */}
          <Section title="Tool Mode">
            <div style={{ display: "flex", gap: 4 }}>
              {[
                { id: "split", label: "✂ Split", color: "#f97316" },
                { id: "opening", label: "▢ Opening", color: "#38bdf8" },
                { id: "select", label: "◉ Select", color: "#a78bfa" },
              ].map((m) => (
                <button key={m.id} onClick={() => { setMode(m.id); setAddingOpening(false); setOpeningStart(null); }}
                  style={{
                    ...btnStyle(mode === m.id ? m.color + "33" : "#1e293b"),
                    flex: 1, fontSize: 11,
                    border: mode === m.id ? `1px solid ${m.color}` : "1px solid #334155",
                    color: mode === m.id ? m.color : "#94a3b8",
                  }}>
                  {m.label}
                </button>
              ))}
            </div>
          </Section>

          {/* Grid */}
          <Section title="Grid & Snap">
            <label style={{ display: "flex", alignItems: "center", gap: 8, fontSize: 11, color: "#94a3b8" }}>
              <input type="checkbox" checked={showGrid} onChange={(e) => setShowGrid(e.target.checked)} />
              Show Grid
            </label>
            <label style={{ display: "flex", alignItems: "center", gap: 8, fontSize: 11, color: "#94a3b8" }}>
              <input type="checkbox" checked={snapToGrid} onChange={(e) => setSnapToGrid(e.target.checked)} />
              Snap to Grid
            </label>
            <InputRow label="Grid (mm)" value={gridSizeMm} onChange={(v) => setGridSizeMm(parseInt(v) || 100)} />
          </Section>

          {/* Actions */}
          <Section title="Quick Actions">
            <button onClick={autoSplit} style={btnStyle("#f97316", true)}>
              ⫽ Auto-Split ({wall.panelWidthMm}mm)
            </button>
            <button onClick={clearAll} style={btnStyle("#ef4444")}>✕ Clear All</button>
            <button onClick={generateSkill} style={btnStyle("#10b981", true)}>
              ✦ Generate Skill
            </button>
          </Section>

          {/* Segment Properties */}
          {selectedSegment !== null && segments[selectedSegment] && (
            <Section title={`Segment ${selectedSegment + 1}`}>
              <div style={{
                width: "100%", height: 4, borderRadius: 2,
                background: segments[selectedSegment].color, marginBottom: 4,
              }} />
              <InputRow label="Label"
                value={segments[selectedSegment].label}
                onChange={(v) => updateSegmentProp(selectedSegment, "label", v)} />
              <div style={{ fontSize: 10, color: "#64748b", marginTop: 4 }}>Profile Shape</div>
              <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: 3, marginTop: 4 }}>
                {PROFILE_SHAPES.map((ps) => (
                  <button key={ps.id}
                    onClick={() => updateSegmentProp(selectedSegment, "profile", ps.id)}
                    style={{
                      ...btnStyle(
                        segments[selectedSegment].profile === ps.id
                          ? segments[selectedSegment].color + "33"
                          : "#1e293b"
                      ),
                      fontSize: 10,
                      border: segments[selectedSegment].profile === ps.id
                        ? `1px solid ${segments[selectedSegment].color}`
                        : "1px solid #334155",
                    }}>
                    <span style={{ fontSize: 14 }}>{ps.icon}</span> {ps.label}
                  </button>
                ))}
              </div>
              <div style={{ fontSize: 10, color: "#64748b", marginTop: 8 }}>Fire Rating</div>
              <select
                value={segments[selectedSegment].fireRating}
                onChange={(e) => updateSegmentProp(selectedSegment, "fireRating", e.target.value)}
                style={{
                  width: "100%", padding: "6px 8px", marginTop: 4,
                  background: "#1e293b", border: "1px solid #334155", borderRadius: 6,
                  color: "#e2e8f0", fontSize: 11, fontFamily: "inherit",
                }}>
                <option value="none">None</option>
                <option value="1hr">1 Hour</option>
                <option value="2hr">2 Hour</option>
                <option value="3hr">3 Hour</option>
              </select>
              <div style={{
                marginTop: 8, padding: "6px 8px", background: "#1e293b",
                borderRadius: 6, fontSize: 10, color: "#94a3b8",
              }}>
                Width: <span style={{ color: "#f8fafc" }}>{segments[selectedSegment].widthMm}mm</span>
                <br />
                Range: {segments[selectedSegment].startMm}–{segments[selectedSegment].endMm}mm
              </div>
            </Section>
          )}

          {/* Summary */}
          <Section title="Summary">
            <div style={{ fontSize: 10, color: "#94a3b8", lineHeight: 1.8 }}>
              Segments: <span style={{ color: "#f8fafc" }}>{segments.length}</span><br />
              Split Points: <span style={{ color: "#f8fafc" }}>{splitPoints.length}</span><br />
              Openings: <span style={{ color: "#f8fafc" }}>{openings.length}</span><br />
              Profiles: <span style={{ color: "#f8fafc" }}>
                {[...new Set(segments.map(s => s.profile))].join(", ")}
              </span>
            </div>
          </Section>
        </div>

        {/* Canvas area */}
        <div style={{ flex: 1, display: "flex", flexDirection: "column" }}>
          <div style={{
            flex: 1, display: "flex", alignItems: "center", justifyContent: "center",
            background: "#080c18",
          }}>
            <svg ref={canvasRef} width={CANVAS_W} height={CANVAS_H}
              style={{ cursor: mode === "split" ? "crosshair" : mode === "opening" ? "cell" : "pointer" }}
              onClick={handleCanvasClick}
              onMouseMove={handleCanvasMouseMove}
              onMouseDown={handleCanvasMouseDown}
              onMouseUp={handleCanvasMouseUp}
              onContextMenu={handleRightClick}>
              <defs>
                <marker id="arrowR" viewBox="0 0 6 6" refX="6" refY="3" markerWidth="6" markerHeight="6" orient="auto">
                  <path d="M0,0 L6,3 L0,6" fill="#64748b" />
                </marker>
                <marker id="arrowL" viewBox="0 0 6 6" refX="0" refY="3" markerWidth="6" markerHeight="6" orient="auto">
                  <path d="M6,0 L0,3 L6,6" fill="#64748b" />
                </marker>
                <marker id="arrowD" viewBox="0 0 6 6" refX="3" refY="6" markerWidth="6" markerHeight="6" orient="auto">
                  <path d="M0,0 L3,6 L6,0" fill="#64748b" />
                </marker>
                <marker id="arrowU" viewBox="0 0 6 6" refX="3" refY="0" markerWidth="6" markerHeight="6" orient="auto">
                  <path d="M0,6 L3,0 L6,6" fill="#64748b" />
                </marker>
              </defs>

              {/* Background */}
              <rect width={CANVAS_W} height={CANVAS_H} fill="#080c18" />

              {/* Grid */}
              {renderGrid()}

              {/* Wall outline */}
              <rect x={offsetX} y={offsetY} width={wallDrawW} height={wallDrawH}
                fill="none" stroke="#475569" strokeWidth={2} rx={1} />

              {/* Segments */}
              {renderSegments()}

              {/* Openings */}
              {renderOpenings()}

              {/* Split lines */}
              {renderSplitPoints()}

              {/* Dimensions */}
              {renderDimensions()}

              {/* Crosshair hint for opening mode */}
              {addingOpening && openingStart && (
                <circle cx={mmToX(openingStart.x)} cy={mmToY(openingStart.y)}
                  r={4} fill="#38bdf8" stroke="#0f172a" strokeWidth={1.5} />
              )}
            </svg>
          </div>

          {/* Segment strip */}
          <div style={{
            height: 56, borderTop: "1px solid #1e293b", padding: "8px 16px",
            display: "flex", gap: 4, alignItems: "center", overflowX: "auto",
            background: "#0d1224",
          }}>
            {segments.map((s, i) => (
              <button key={i}
                onClick={() => { setSelectedSegment(i); setMode("select"); }}
                style={{
                  padding: "6px 12px", borderRadius: 6,
                  background: selectedSegment === i ? s.color + "25" : "#1e293b",
                  border: selectedSegment === i ? `1px solid ${s.color}` : "1px solid #334155",
                  color: selectedSegment === i ? s.color : "#94a3b8",
                  fontSize: 10, fontFamily: "inherit", cursor: "pointer",
                  whiteSpace: "nowrap",
                  transition: "all 0.15s",
                }}>
                <span style={{
                  display: "inline-block", width: 8, height: 8,
                  borderRadius: 2, background: s.color, marginRight: 6,
                }} />
                {s.label} · {s.widthMm}mm · {s.profile}
              </button>
            ))}
          </div>
        </div>

        {/* Skill output panel */}
        {showSkillPanel && generatedSkill && (
          <div style={{
            width: 340, borderLeft: "1px solid #1e293b", padding: 16,
            overflowY: "auto", background: "#0d1224",
          }}>
            <div style={{
              display: "flex", justifyContent: "space-between", alignItems: "center",
              marginBottom: 12,
            }}>
              <div style={{ fontSize: 13, fontWeight: 700, color: "#10b981" }}>
                ✦ Generated Skill
              </div>
              <button onClick={() => setShowSkillPanel(false)}
                style={{ ...btnStyle("#334155"), padding: "2px 8px", fontSize: 11 }}>
                ✕
              </button>
            </div>
            <div style={{ fontSize: 10, color: "#64748b", marginBottom: 8 }}>
              This JSON defines your wall split standard. It can be stored and reused
              by the execution engine.
            </div>
            <pre style={{
              background: "#0f172a", border: "1px solid #1e293b",
              borderRadius: 8, padding: 12, fontSize: 10,
              color: "#a5f3fc", overflow: "auto", maxHeight: "calc(100vh - 200px)",
              lineHeight: 1.6, whiteSpace: "pre-wrap", wordBreak: "break-all",
            }}>
              {JSON.stringify(generatedSkill, null, 2)}
            </pre>
            <button onClick={() => navigator.clipboard?.writeText(JSON.stringify(generatedSkill, null, 2))}
              style={{ ...btnStyle("#334155"), marginTop: 8, width: "100%", fontSize: 11 }}>
              Copy to Clipboard
            </button>
          </div>
        )}
      </div>
    </div>
  );
}

// --- Shared sub-components ---
function Section({ title, children }) {
  return (
    <div>
      <div style={{
        fontSize: 10, fontWeight: 700, color: "#64748b",
        textTransform: "uppercase", letterSpacing: "0.08em",
        marginBottom: 8,
      }}>
        {title}
      </div>
      <div style={{ display: "flex", flexDirection: "column", gap: 6 }}>
        {children}
      </div>
    </div>
  );
}

function InputRow({ label, value, onChange }) {
  return (
    <div style={{ display: "flex", alignItems: "center", gap: 8 }}>
      <span style={{ fontSize: 10, color: "#94a3b8", minWidth: 70 }}>{label}</span>
      <input value={value}
        onChange={(e) => onChange(e.target.value)}
        style={{
          flex: 1, padding: "5px 8px",
          background: "#1e293b", border: "1px solid #334155", borderRadius: 6,
          color: "#f8fafc", fontSize: 11, fontFamily: "inherit",
          outline: "none",
        }}
        onFocus={(e) => e.target.style.borderColor = "#f97316"}
        onBlur={(e) => e.target.style.borderColor = "#334155"}
      />
    </div>
  );
}

function btnStyle(bg, bold) {
  return {
    padding: "7px 12px", borderRadius: 6,
    background: bg, border: "none",
    color: "#e2e8f0", fontSize: 11, fontFamily: "inherit",
    cursor: "pointer", fontWeight: bold ? 700 : 400,
    transition: "all 0.15s",
  };
}

export default WallProfileEditor;
