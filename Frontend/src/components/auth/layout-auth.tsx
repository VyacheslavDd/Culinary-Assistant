import styles from './layout-auth.module.scss';

export function LayoutAuth({ children }: { children: React.ReactNode }) {
    return (
        <div className={styles.mainContainer}>
            <div className={styles.container}>
                <a href='/' className={styles.back}>
                    <span className={styles.arrow}></span>
                    На главную
                </a>
                {children}
            </div>
        </div>
    );
}
