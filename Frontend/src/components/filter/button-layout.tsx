import React, { useState, useRef, useEffect } from 'react';
import styles from './button-layout.module.scss';
import ingredient_icon from '../../assets/svg/ingredients_icon.svg';
import fiter_icon from '../../assets/svg/filter_icon.svg';
import sort_icon from '../../assets/svg/sort_icon.svg';

type IconName = 'ingredients' | 'filter' | 'sort';

type Props = {
    children: React.ReactNode;
    color: 'white' | 'blue';
    name: string;
    icon: IconName;
};

const icons: Record<IconName, string> = {
    ingredients: ingredient_icon,
    filter: fiter_icon,
    sort: sort_icon,
};

export function ButtonLayout(props: Props) {
    const { children, color, name, icon } = props;
    const [open, setOpen] = useState(false);
    const dropdownRef = useRef<HTMLDivElement>(null);

    const toggleMenu = () => setOpen(!open);

    useEffect(() => {
        const handleClickOutside = (event: MouseEvent) => {
            if (
                dropdownRef.current &&
                !dropdownRef.current.contains(event.target as Node)
            ) {
                setOpen(false);
            }
        };
        document.addEventListener('mousedown', handleClickOutside);
        return () => {
            document.removeEventListener('mousedown', handleClickOutside);
        };
    }, []);
    return (
        <div className={styles.mainContainer} ref={dropdownRef}>
            <button
                onClick={toggleMenu}
                className={`${styles.button} ${styles[color]}`}
            >
                <img src={icons[icon]} alt={name} className={styles.icon} />
                {name}
            </button>

            {open && (
                <div className={styles.dropdown}>
                    {children}
                    <button className={`${styles.button} ${styles.apply}`}>
                        Применить
                    </button>
                </div>
            )}
        </div>
    );
}
