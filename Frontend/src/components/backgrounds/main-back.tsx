import styles from './main-back.module.scss';

function MainBack() {
    return (
        <div className={styles.container}>
            <div className={styles.text}>
                <h1 className={styles.title}>КулиНорка</h1>
                <div className={styles.description}>
                    <p>
                        Ищете что приготовить для особого случая или что-нибудь
                        простое на каждый день? Имеете особые вкусовые
                        предпочтения и никто не может Вам угодить? Сходили в
                        магазин и не знаете, что приготовить?{' '}
                        <strong>
                            Так много ситуаций, а решение одно - мы!
                        </strong>
                    </p>
                    <p>Собрали для Вас все рецепты в одном месте! Смотри ниже<span className={styles.arrow}></span></p>
                </div>
                <p className={styles.copyright}>Разработано командой «Бебрики»</p>
            </div>
        </div>
    );
}

export default MainBack;
