/** @type {import('tailwindcss').Config} */
export default {
  content: ['./index.html', './src/**/*.{js,jsx}'],
  theme: {
    extend: {
      colors: {
        ink: {
          DEFAULT: '#12151B',
          soft: '#171B23',
        },
        panel: {
          DEFAULT: '#1B1F28',
          raised: '#232838',
        },
        line: '#2B3140',
        paper: {
          DEFAULT: '#DCD3B9',
          dim: '#B9AF95',
        },
        ink2: '#0E1116',
        brass: {
          DEFAULT: '#C9A24A',
          bright: '#E0BE6C',
          dim: '#8C7433',
        },
        stamp: {
          approved: '#5B9A6F',
          approvedDim: '#3E6B4C',
          rejected: '#B85C52',
          rejectedDim: '#7E3F38',
          pending: '#C9A24A',
          reviewed: '#6E8FB0',
        },
        scan: '#6FCFC0',
        ivory: '#EDEAE0',
        muted: '#93998F',
        mutedCool: '#8992A3',
      },
      fontFamily: {
        display: ['"Fraunces"', 'serif'],
        body: ['"Inter"', 'sans-serif'],
        mono: ['"IBM Plex Mono"', 'monospace'],
      },
      boxShadow: {
        card: '0 1px 0 rgba(220,211,185,0.06), 0 8px 24px -12px rgba(0,0,0,0.6)',
        seal: 'inset 0 0 0 1px rgba(201,162,74,0.35)',
      },
      backgroundImage: {
        grain: "radial-gradient(circle at 1px 1px, rgba(220,211,185,0.05) 1px, transparent 0)",
      },
    },
  },
  plugins: [],
}
