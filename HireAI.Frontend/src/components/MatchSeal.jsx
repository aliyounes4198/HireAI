// The app's signature element: a rotated, rubber-stamp-style seal that
// reports the AI match score, echoing the "case file" motif used throughout.
export default function MatchSeal({ score = 0, size = 96, label = 'AI MATCH' }) {
  const tone =
    score >= 75 ? '#5B9A6F' : score >= 45 ? '#C9A24A' : '#B85C52'

  const r = 40
  const circumference = 2 * Math.PI * r
  const offset = circumference - (Math.min(100, Math.max(0, score)) / 100) * circumference

  return (
    <div
      className="relative shrink-0 select-none"
      style={{ width: size, height: size, transform: 'rotate(-6deg)' }}
    >
      <svg viewBox="0 0 100 100" width={size} height={size}>
        <circle cx="50" cy="50" r="46" fill="none" stroke="#2B3140" strokeWidth="1" strokeDasharray="1 3" />
        <circle cx="50" cy="50" r={r} fill="none" stroke="#232838" strokeWidth="6" />
        <circle
          cx="50"
          cy="50"
          r={r}
          fill="none"
          stroke={tone}
          strokeWidth="6"
          strokeLinecap="round"
          strokeDasharray={circumference}
          strokeDashoffset={offset}
          transform="rotate(-90 50 50)"
          style={{ transition: 'stroke-dashoffset 700ms ease' }}
        />
        <text
          x="50"
          y="47"
          textAnchor="middle"
          className="font-mono"
          fontSize="22"
          fill="#EDEAE0"
          fontWeight="600"
        >
          {score}
        </text>
        <text x="50" y="62" textAnchor="middle" className="font-mono" fontSize="7.5" fill="#93998F" letterSpacing="1">
          {label}
        </text>
      </svg>
    </div>
  )
}
