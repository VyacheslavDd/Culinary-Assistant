type Props = {
    isActive: boolean;
    onToggleChange: (newPrivate: boolean) => void;
};

export function Toggle(props: Props) {
    const { isActive, onToggleChange } = props;

    const handleClick = () => {
        onToggleChange(isActive);
    };

    return isActive ? (
        <button
            style={{ backgroundColor: 'transparent', border: 'none' }}
            onClick={handleClick}
        >
            <svg
                xmlns='http://www.w3.org/2000/svg'
                width='44'
                height='24'
                viewBox='0 0 44 24'
                fill='none'
                style={{ cursor: 'pointer' }}
            >
                <rect width='44' height='24' rx='12' fill='#91A6FF' />
                <circle cx='32' cy='12' r='10' fill='#FEF9EF' />
            </svg>
        </button>
    ) : (
        <button
            style={{ backgroundColor: 'transparent', border: 'none' }}
            onClick={handleClick}
        >
            <svg
                xmlns='http://www.w3.org/2000/svg'
                width='44'
                height='24'
                viewBox='0 0 44 24'
                fill='none'
                style={{ cursor: 'pointer' }}
            >
                <rect width='44' height='24' rx='12' fill='#B1B1B1' />
                <circle cx='12' cy='12' r='10' fill='#FEF9EF' />
            </svg>
        </button>
    );
}
