import styles from './search.module.scss';
import search from '../../../assets/svg/search.svg';

type props = {
    onClick: () => void;
    onChange: (e: React.ChangeEvent<HTMLInputElement>) => void;
    value: string;
    isFindShows?: boolean;
};

export function Search(props: props) {
    const { onClick, onChange, value, isFindShows = true } = props;

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        onChange(e);
    };

    const handleKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) => {
        if (e.key === 'Enter') {
            onClick();
        }
    };

    const handleClick = (e: React.MouseEvent<HTMLButtonElement>) => {
        e.preventDefault();
        onClick();
    };

    return (
        <div className={styles.mainContainer}>
            <img src={search} alt='search' className={styles.icon} />

            <input
                type='text'
                placeholder='Что хотите найти?'
                className={styles.input}
                value={value}
                onChange={handleChange}
                onKeyDown={handleKeyDown}
            />
            {isFindShows && (
                <button className={styles.button} onClick={handleClick}>
                    Найти
                </button>
            )}
        </div>
    );
}
