import styles from './main-back.module.scss';

function MainBack() {
    return (
        <div className={styles.container}>
            <div className={styles.text}>
                <h1 className={styles.title}>КулиНорка</h1>
                <div className={styles.description}>
                    <p>
                        Лучший сервис для любителей готовить! Собрали для Вас разнообразные рецепты на любой вкус в одном месте.
                    </p>
                    <p>Смотри ниже<span className={styles.arrow}></span></p>
                </div>
                <p className={styles.copyright}>Разработано командой «Бебрики»</p>
            </div>
        </div>
    );
}

export default MainBack;
